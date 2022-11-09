
using System;
using System.Collections.Generic;
using System.Xml;


//Lookup for SSP commands, generic responses and poll responses.
class CSspLookup
{
    //Command struct.  If command has fixed length, that length will be held in .Length
    //If command has variable length, .Length will be 0 and byte at position given by .LengthByte
    //should be read, multiplied by .Multiply and have .Add added to give the length of the
    //command. Variable length commands facility is not used at the moment.  
    private struct SspCommand
    {
        public string CommandName;
        public int Length;
        public int LengthByte;
        public int Multiply;
        public int Add;
    }
    //Poll response struct.  If poll response has fixed length, that length will be held in .Length
    //If poll response has variable length, .Length will be 0 and byte at position in data block given by .LengthByte
    //should be read, multiplied by .Multiply and have .Add added to give the length of the
    //poll response. 
    private struct SspPollResponse
    {
        public string ResponseName;
        public int Length;
        public int LengthByte;
        public int Multiply;
        public int Add;
    }
    //Generic response struct.  Struct used rather than a string for possible future expansion.
    private struct SspGenericResponse
    {
        public string ResponseName;
    }

    //Dictionaries to contain data read from XML files.
    private Dictionary<int, SspCommand> commandsDictionary;
    private Dictionary<int, SspPollResponse> pollResponsesDictionary;
    private Dictionary<int, SspGenericResponse> genericResponsesDictionary;

    //Constructor
    public CSspLookup()
    {
        CSspReadCommandsData();
        CSspReadGenericResponsesData();
        CSspReadPollResponsesData();
    }

    //Takes commandCode and returns CommandName from dictionary
    public string GetCommandName(int commandCode)
    {
        SspCommand SspCom = new SspCommand();
        if (commandsDictionary.ContainsKey(commandCode))
        {
            SspCom = commandsDictionary[commandCode];
            return SspCom.CommandName;
        }
        return "Unknown Command";
    }

    //Takes responseCode and returns ResponseName from dictionary
    public string GetGenericResponseName(int responseCode)
    {
        SspGenericResponse SspGenRep = new SspGenericResponse();
        if (genericResponsesDictionary.ContainsKey(responseCode))
        {
            SspGenRep = genericResponsesDictionary[responseCode];
            return SspGenRep.ResponseName;
        }
        return "Unknown Response";
    }

    //Takes response to poll command as a byte array, parses out individual responses and returns
    //a string containing the response names
    public string GetPollResponse(byte[] response)
    {
        string ReturnString = "";
        int ResponseCode = 0;
        int Increment = 0;
        SspPollResponse SspPolResp = new SspPollResponse();
        try
        {
            //Start at first byte
            int ByteCounter = 1;
            //Loop until end of byte array is reached
            while (ByteCounter < response.Length)
            {
                //Add the ResponseName for the ResponseCode to the return string.
                ResponseCode = response[ByteCounter];
                SspPolResp = pollResponsesDictionary[ResponseCode];
                ReturnString += SspPolResp.ResponseName;
               
                //If response is fixed length
                if (SspPolResp.Length != 0)
                {
                    //Increment byte counter by fixed amount
                    Increment = SspPolResp.Length;
                }
                else
                {
                    //Else calculate length of response from data and increment byte counter
                    Increment = response[(ByteCounter + SspPolResp.LengthByte)];
                    Increment *= SspPolResp.Multiply;
                    Increment += SspPolResp.Add;
                    Increment += 1;
                }

                ByteCounter += Increment;

                if (ByteCounter < response.Length)
                {
                    ReturnString += ", ";
                }
            }
        }
        catch (Exception e)
        {
            ReturnString = e.Message;
        }
        ReturnString += "\r\n";
        return ReturnString;
    }

    //Reads data from Resources/Commands.xml
    private void CSspReadCommandsData()
    {
        commandsDictionary = new Dictionary<int, SspCommand>();
        XmlDocument document = new XmlDocument();
        SspCommand SspCom = new SspCommand();
        int i = 0;
        document.Load("Resources/Commands.xml");

        XmlNodeList nodeList = document.DocumentElement.SelectNodes("/Root/CommandInfo");

        //Loop through all CommandInfo nodes in /Root.
        foreach (XmlNode node in nodeList)
        {
            //The xml document contains a CommandInfo node for every possible (0x00 to 0xFF) value of CommandCode.
            //Only process nodes witha a <"CommandName"/> child
            if (node.SelectSingleNode("CommandName") != null)
            {
                i = Int32.Parse(node.SelectSingleNode("CommandCode").InnerText);

                //Put CommandName into struct 
                SspCom.CommandName = node.SelectSingleNode("CommandName").InnerText;

                //Add any other elements, if present in xml
                if (node.SelectSingleNode("Length") != null)
                {
                    SspCom.Length = Int32.Parse(node.SelectSingleNode("Length").InnerText);
                }
                else
                {
                    SspCom.Length = 0;
                }

                if (node.SelectSingleNode("LengthByte") != null)
                {
                    SspCom.LengthByte = Int32.Parse(node.SelectSingleNode("LengthByte").InnerText);
                }
                else
                {
                    SspCom.LengthByte = 0;
                }

                if (node.SelectSingleNode("Multiply") != null)
                {
                    SspCom.Multiply = Int32.Parse(node.SelectSingleNode("Multiply").InnerText);
                }
                else
                {
                    SspCom.Multiply = 0;
                }

                if (node.SelectSingleNode("Add") != null)
                {
                    SspCom.Add = Int32.Parse(node.SelectSingleNode("Add").InnerText);
                }
                else
                {
                    SspCom.Add = 0;
                }
                //Add entry to dictionary
                commandsDictionary.Add(i, SspCom);
            }
        }
    }
    //Reads data from Resources/GenericResponses.xml
    private void CSspReadGenericResponsesData()
    {
        genericResponsesDictionary = new Dictionary<int, SspGenericResponse>();
        XmlDocument document = new XmlDocument();
        SspGenericResponse SspGenericResponse = new SspGenericResponse();
        int i = 0;
        document.Load("Resources/GenericResponses.xml");

        XmlNodeList nodeList = document.DocumentElement.SelectNodes("/Root/GenericResponseInfo");
        //Loop through all GenericResponseInfo nodes in /Root.
        foreach (XmlNode node in nodeList)
        {
            //The xml document contains a GenericResponseInfo node for every possible (0x00 to 0xFF) value of GenericResponseCode.
            //Only process nodes witha a <"GenericResponseName"/> child
            if (node.SelectSingleNode("GenericResponseName") != null)
            {
                //Add GenericResponseCode and GenericResponseName into the dictionary.
                i = Int32.Parse(node.SelectSingleNode("GenericResponseCode").InnerText);
                SspGenericResponse.ResponseName = node.SelectSingleNode("GenericResponseName").InnerText;
                genericResponsesDictionary.Add(i, SspGenericResponse);
            }
        }
    }

    //Reads data from Resources/PollResponses.xml.
    private void CSspReadPollResponsesData()
    {
        pollResponsesDictionary = new Dictionary<int, SspPollResponse>();
        XmlDocument document = new XmlDocument();
        SspPollResponse SspPoll = new SspPollResponse();
        int i = 0;
        document.Load("Resources/PollResponses.xml");

        XmlNodeList nodeList = document.DocumentElement.SelectNodes("/Root/PollResponseInfo");

        //Loop through all PollResponseInfo nodes in /Root.
        foreach (XmlNode node in nodeList)
        {

            if (node.SelectSingleNode("PollResponseName") != null)
            {
                i = Int32.Parse(node.SelectSingleNode("PollResponseCode").InnerText);

                //Put PollResponseName into struct 
                SspPoll.ResponseName = node.SelectSingleNode("PollResponseName").InnerText;

                //Add any other elements, if present in xml
                if (node.SelectSingleNode("Length") != null)
                {
                    SspPoll.Length = Int32.Parse(node.SelectSingleNode("Length").InnerText);
                }
                else
                {
                    SspPoll.Length = 0;
                }

                if (node.SelectSingleNode("LengthByte") != null)
                {
                    SspPoll.LengthByte = Int32.Parse(node.SelectSingleNode("LengthByte").InnerText);
                }
                else
                {
                    SspPoll.LengthByte = 0;
                }

                if (node.SelectSingleNode("Multiply") != null)
                {
                    SspPoll.Multiply = Int32.Parse(node.SelectSingleNode("Multiply").InnerText);
                }
                else
                {
                    SspPoll.Multiply = 0;
                }

                if (node.SelectSingleNode("Add") != null)
                {
                    SspPoll.Add = Int32.Parse(node.SelectSingleNode("Add").InnerText);
                }
                else
                {
                    SspPoll.Add = 0;
                }
                //Add entry to dictionary
                pollResponsesDictionary.Add(i, SspPoll);
            }
        }
    }
}