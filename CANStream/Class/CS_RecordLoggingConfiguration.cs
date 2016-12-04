﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CANStream
{
    #region Public enums

    /// <summary>
    /// Enumeration of logging mode for a particular channel
    /// </summary>
    public enum ChannelLoggingMode
    {
        DefaultFrequency = 0,   /// <summary>Logging frequency is the same as PCAN Trace file</summary>
        CustomFrequency = 1,    /// <summary>Logging frequency is defined by the user</summary>
        NotLogged = 2,          /// <summary>Channel is not logged and won't appear in the record data file</summary>
    }

    #endregion

    #region Public sub class

    /// <summary>
    /// Record data file channel logging configuration class
    /// </summary>
    [Serializable]
    public class LoggingChannelConfiguration
    {
        #region Public properties

        /// <summary>Name of the logging channel</summary>
        public string Name { get; set; }

        /// <summary> Full path of the current LoggingChannelConfiguration within the group hierarchy</summary>
        public string Path { get; set; }

        /// <summary>Logging mode of the logging channel</summary>
        public ChannelLoggingMode LoggingMode { get; set; }

        /// <summary>Logging frequency of the logging channel</summary>
        public double LoggingFrequency { get; set; }

        /// <summary>Default Logging frequency of the logging channel</summary>
        public double DefaultFrequency { get; set; }

        /// <summary>Description of the logging channel</summary>
        public string Comment { get; set; }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoggingChannelConfiguration()
        {
            Name = "";
            Path = "";
            LoggingMode = ChannelLoggingMode.DefaultFrequency;
            LoggingFrequency = 0;
            DefaultFrequency = 0;
            Comment = "";
        }

        #region Public methodes

        /// <summary>
        /// Returns a clone of the current LoggingChannelConfiguration object
        /// </summary>
        /// <returns>Clone of the current LoggingChannelConfiguration object</returns>
        public LoggingChannelConfiguration Clone()
        {
            LoggingChannelConfiguration oClone = new LoggingChannelConfiguration();

            oClone.Comment = this.Comment;
            oClone.DefaultFrequency = this.DefaultFrequency;
            oClone.LoggingFrequency = this.LoggingFrequency;
            oClone.LoggingMode = this.LoggingMode;
            oClone.Name = this.Name;
            oClone.Path = this.Path;

            return (oClone);
        }

        #endregion
    }

    /// <summary>
    /// Channel logging configuration group class
    /// </summary>
    [Serializable]
    public class LoggingChannelGroup
    {
        #region Public properties

        /// <summary>
        /// Group name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Full path of the current LoggingChannelGroup within the group hierarchy
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Logging mode set for all logging channels and sub group of the current group
        /// </summary>
        public ChannelLoggingMode GroupLoggingMode { get; set; }

        /// <summary>
        /// Logging frequency set for all logging channels and sub group of the current group
        /// </summary>
        public double GroupLoggingFrequency { get; set; }

        /// <summary>
        /// Default logging frequency set for all logging channels and sub group of the current group
        /// </summary>
        public double GroupDefaultFrequency { get; set; }

        /// <summary>
        /// Description of the logging channel
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Logging channels contained in the current group
        /// </summary>
        public List<LoggingChannelConfiguration> LoggingChannels { get; set; }

        /// <summary>
        /// Sub logging channel groups of the current group
        /// </summary>
        public List<LoggingChannelGroup> SubGroups { get; set; }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoggingChannelGroup()
        {
            Name = "";
            FullPath = "";
            GroupLoggingMode = ChannelLoggingMode.DefaultFrequency;
            GroupLoggingFrequency = 0;
            GroupDefaultFrequency = 0;
            Comment = "";
            LoggingChannels = new List<LoggingChannelConfiguration>();
            SubGroups = new List<LoggingChannelGroup>();
        }

        #region Public methodes

        /// <summary>
        /// Returns the first level child sub group corresponding to the group name given as argument
        /// </summary>
        /// <param name="GroupName">Name of the group to retrieve</param>
        /// <returns>LoggingChannelGroup object corresponding to the group name given as argument</returns>
        /// <remarks>Returns null if the group is not found</remarks>
        public LoggingChannelGroup Get_SubGroup(string GroupName)
        {
            foreach (LoggingChannelGroup oGroup in this.SubGroups)
            {
                if(oGroup.Name.Equals(GroupName))
                {
                    return (oGroup);
                }
            }

            return (null);
        }

        /// <summary>
        /// Return the LoggingChannelGroup object corresponding to the group path given as argument
        /// </summary>
        /// <param name="GroupPath">Path of the LoggingChannelGroup object to retrive</param>
        /// <returns>LoggingChannelGroup object corresponding to the group path given as argument</returns>
        /// <remarks>Return null if the group is not found</remarks>
        public LoggingChannelGroup Get_GroupAtPath(string GroupPath)
        {
            string[] GroupNames = GroupPath.Split('\\');
            LoggingChannelGroup oCurrentGroup = this;

            if (GroupNames.Length == 1)
            {
                return (oCurrentGroup);
            }
            else if (GroupNames.Length > 1)
            {
                for (int i = 1 ; i < GroupNames.Length; i++)
                {
                    LoggingChannelGroup oSubGroup = oCurrentGroup.Get_SubGroup(GroupNames[i]);

                    if (!(oSubGroup == null))
                    {
                        if (i < GroupNames.Length - 1)
                        {
                            oCurrentGroup = oSubGroup;
                        }
                        else
                        {
                            return (oSubGroup);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return (null);
        }

        /// <summary>
        /// Return the LoggingChannelGroup object corresponding to the name given as argument looking recursively into sub groups hierarchy of the current group
        /// </summary>
        /// <param name="GroupName">Name of the group to retrieve</param>
        /// <returns>LoggingChannelGroup object corresponding to the name given as argument</returns>
        /// <remarks>Returns null if the group is not found</remarks>
        public LoggingChannelGroup Get_TreeSubGroup(string GroupName)
        {
            LoggingChannelGroup oGroup = Get_SubGroup(GroupName);

            if (oGroup == null)
            {
                foreach(LoggingChannelGroup oSubGroup in SubGroups)
                {
                    oGroup = oSubGroup.Get_TreeSubGroup(GroupName);
                    if (!(oGroup == null)) return (oGroup);
                }
            }
            else
            {
                return (oGroup);
            }

            return (null);
        }

        /// <summary>
        /// Returns the logging channel object corresponding to the name given as argument within logging channels of the curent group
        /// </summary>
        /// <param name="ChannelName">Name of the logging channel object to retrieve</param>
        /// <returns>Logging channel object corresponding to the name given as argument</returns>
        /// <remarks>Return null if the Logging channel is not found</remarks>
        public LoggingChannelConfiguration Get_LoggingChannel(string ChannelName)
        {
            foreach (LoggingChannelConfiguration oChannel in LoggingChannels)
            {
                if (oChannel.Name.Equals(ChannelName))
                {
                    return (oChannel);
                }
            }

            return (null);
        }

        /// <summary>
        /// Returns the logging channel object corresponding to the name given as argument looking recursively into sub groups hierarchy of the current group
        /// </summary>
        /// <param name="ChannelName">Name of the logging channel object to retrieve</param>
        /// <returns>Logging channel object corresponding to the name given as argument</returns>
        /// <remarks>Return null if the Logging channel is not found</remarks>
        public LoggingChannelConfiguration Get_TreeLoggingChannel(string ChannelName)
        {
            LoggingChannelConfiguration oChannel = this.Get_LoggingChannel(ChannelName);

            if (oChannel == null)
            {
                foreach (LoggingChannelGroup oSubGroup in this.SubGroups)
                {
                    oChannel = oSubGroup.Get_TreeLoggingChannel(ChannelName);

                    if (!(oChannel == null))
                    {
                        return (oChannel);
                    }
                }
            }
            else
            {
                return (oChannel);
            }

            return (null);
        }

        /// <summary>
        /// Returns all pathes of all sub groups looking recursively into sub groups hierarchy of the current group
        /// </summary>
        /// <returns>Pathes of sub groups hierarchy</returns>
        public List<string> Get_FullGroupsPathesList()
        {
            List<string> Pathes = new List<string>();

            foreach(LoggingChannelGroup oSubGroup in this.SubGroups)
            {
                Pathes.Add(oSubGroup.FullPath);

                List<string> SubGroupPathes = oSubGroup.Get_FullGroupsPathesList();

                if(SubGroupPathes.Count>0)
                {
                    Pathes.AddRange(SubGroupPathes);
                }
            }

            return (Pathes);
        }

        /// <summary>
        /// Returns all LoggingChannel objects of the current group including all sub groups looking recursively into sub groups hierarchy of the current group
        /// </summary>
        /// <returns>LoggingChannel objects list of the group hierarchy</returns>
        public List<LoggingChannelConfiguration> Get_FullChannelHierarchy()
        {
            List<LoggingChannelConfiguration> oChannelList = new List<LoggingChannelConfiguration>();

            oChannelList.AddRange(this.LoggingChannels);

            foreach (LoggingChannelGroup oSubGroup in this.SubGroups)
            {
                List<LoggingChannelConfiguration> oSubGroupChannels = oSubGroup.Get_FullChannelHierarchy();

                if (oSubGroupChannels.Count > 0)
                {
                    oChannelList.AddRange(oSubGroupChannels);
                }
            }

            return (oChannelList);
        }

        /// <summary>
        /// Returns the LoggingChannel object count within the sub group hierarchy of the current group
        /// </summary>
        /// <returns>LoggingChannel object count</returns>
        public int Get_HierarchyLoggingChannelsCount()
        {
            int ChannelsCount = 0;

            ChannelsCount += this.LoggingChannels.Count;

            foreach(LoggingChannelGroup oSubGroup in this.SubGroups)
            {
                ChannelsCount += oSubGroup.Get_HierarchyLoggingChannelsCount();
            }

            return (ChannelsCount);
        }

        /// <summary>
        /// Returns a clone of the current LoggingChannelGroup object
        /// </summary>
        /// <returns>Clone of the current LoggingChannelGroup object</returns>
        public LoggingChannelGroup Clone()
        {
            LoggingChannelGroup oClone = new LoggingChannelGroup();

            oClone.Comment = this.Comment;
            oClone.FullPath = this.FullPath;
            oClone.GroupDefaultFrequency = this.GroupDefaultFrequency;
            oClone.GroupLoggingFrequency = this.GroupLoggingFrequency;
            oClone.GroupLoggingMode = this.GroupLoggingMode;
            oClone.Name = this.Name;

            foreach(LoggingChannelGroup oSubGroup in this.SubGroups)
            {
                oClone.SubGroups.Add(oSubGroup.Clone());
            }

            foreach(LoggingChannelConfiguration oChan in this.LoggingChannels)
            {
                oClone.LoggingChannels.Add(oChan.Clone());
            }

            return (oClone);
        }

        #endregion
    }

    #endregion

    /// <summary>
    /// Record logging configuration class
    /// </summary>
    public class CS_RecordLoggingConfiguration
    {
        #region Public porperties

        public int ChannelsCount
        {
            get
            {
                return (oRootGroup.Get_HierarchyLoggingChannelsCount());
            }
        }

        public List<LoggingChannelGroup> Groups
        {
            get
            {
                return (oRootGroup.SubGroups);
            }
        }

        #endregion

        #region Private members

        /// <summary>
        /// Root group of the current CS_RecordLoggingConfiguration object
        /// </summary>
        private LoggingChannelGroup oRootGroup;

        /// <summary>
        /// File path of the current CS_RecordLoggingConfiguration class storage file
        /// </summary>
        private string LoggingConfigPath;

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public CS_RecordLoggingConfiguration()
        {
            LoggingConfigPath = "";

            oRootGroup = new LoggingChannelGroup();
            oRootGroup.Name = "Root";
            oRootGroup.FullPath = "Root";
        }

        #region Private methodes

        /// <summary>
        /// Returns the path of the parent object from the child object path given as argument
        /// </summary>
        /// <param name="ChildPath">Child object path</param>
        /// <returns>Path of the parent object</returns>
        /// <remarks>Returns an empty string if the path of the parent is not found</remarks>
        private string Get_ParentPath(string ChildPath)
        {
            if (!(ChildPath.Equals("")))
            {
                int i = ChildPath.LastIndexOf(Char.Parse("\\"));

                if (i > 0)
                {
                    return (ChildPath.Substring(0, i));
                }
            }

            return ("");
        }

        /// <summary>
        /// Returns the name of an object from its path
        /// </summary>
        /// <param name="ObjectPath">Path of the object</param>
        /// <returns>Name of the object</returns>
        /// <remarks>Returns an empty string if the object name is not found</remarks>
        private string Get_ObjectNameFromPath(string ObjectPath)
        {
            if (!(ObjectPath.Equals("")))
            {
                int i = ObjectPath.LastIndexOf('\\', 0);

                if (i > 0)
                {
                    i += 1;
                    return (ObjectPath.Substring(i, ObjectPath.Length - i));
                }
            }

            return ("");
        }

        #endregion

        #region Public methodes

        #region Logging channels management

        /// <summary>
        /// Add a LoggingChannelConfiguration object to the root group of the current RecordLoggingConfiguration object
        /// </summary>
        /// <param name="Channel">LoggingChannelConfiguration object to be added</param>
        /// <returns>Channel addition no error flag: True = No Error / False = Error</returns>
        public bool Add_LoggingChannel(LoggingChannelConfiguration Channel)
        {
            return (Add_LoggingChannel(Channel, oRootGroup));
        }

        /// <summary>
        /// Add a LoggingChannelConfiguration object to the group path given as argumennt of the current RecordLoggingConfiguration object
        /// </summary>
        /// <param name="Channel">LoggingChannelConfiguration object to be added</param>
        /// <param name="GroupPath">Path of LoggingChannelGroup in which the channel will be added</param>
        /// <returns>Channel addition no error flag: True = No Error / False = Error</returns>
        public bool Add_LoggingChannel(LoggingChannelConfiguration Channel, string GroupPath)
        {
            LoggingChannelGroup oSubGroup = oRootGroup.Get_GroupAtPath(GroupPath);

            if (!(oSubGroup == null))
            {
                return (Add_LoggingChannel(Channel, oSubGroup));
            }

            return (false);
        }

        /// <summary>
        /// Add a LoggingChannelConfiguration object to the LoggingChannelGroup object given as argument of the current RecordLoggingConfiguration object
        /// </summary>
        /// <param name="Channel">LoggingChannelConfiguration object to be added</param>
        /// <param name="DestGoup">LoggingChannelGroup object reference in which the channel will be added</param>
        /// <returns>Channel addition no error flag: True = No Error / False = Error</returns>
        public bool Add_LoggingChannel(LoggingChannelConfiguration Channel, LoggingChannelGroup DestGoup)
        {
            if (!(Channel == null || DestGoup == null))
            {
                if (DestGoup.Get_LoggingChannel(Channel.Name) == null)
                {
                    Channel.Path = DestGoup.FullPath;
                    DestGoup.LoggingChannels.Add(Channel);

                    return (true);
                }
            }

            return (false);
        }

        /// <summary>
        /// Returns the LoggingChannelConfiguration object from the logging channel configuration list corresponding to the name given as argument
        /// </summary>
        /// <param name="ChannelName">Name of the LoggingChannelConfiguration object to search</param>
        /// <returns>LoggingChannelConfiguration object corresponding to the name given as argument</returns>
        /// <remarks>Returns null if the LoggingChannelConfiguration object is not found</remarks>
        public LoggingChannelConfiguration Get_LoggingChannel(string ChannelName)
        {
            return (oRootGroup.Get_TreeLoggingChannel(ChannelName));
        }

        /// <summary>
        /// Delete the LoggingChannelConfiguration object at the path given as argument
        /// </summary>
        /// <param name="ChannelPath">Path of the LoggingChannelConfiguration object to delete</param>
        /// <param name="ChannelName">Name of the LoggingChannelConfiguration object to delete</param>
        /// <returns>Error flag: True: No error /False: Error</returns>
        public bool Delete_LoggingChannel(string ChannelPath, string ChannelName)
        {
            if (!(ChannelPath.Equals("") || ChannelName.Equals("")))
            {
                LoggingChannelGroup oParentGroup = oRootGroup.Get_GroupAtPath(ChannelPath);

                if (!(oParentGroup == null))
                {
                    LoggingChannelConfiguration oChan = oParentGroup.Get_LoggingChannel(ChannelName);

                    if (!(oChan == null))
                    {
                        oParentGroup.LoggingChannels.Remove(oChan);
                        return (true);
                    }
                }
            }

            return (false);
        }

        #endregion

        #region LoggingChannels groups management

        /// <summary>
        /// Add a logging channel group to the root group of the current RecordLoggingConfiguration object
        /// </summary>
        /// <param name="GroupName">Name of the group to add</param>
        /// <returns>>Group addition no error flag: True = No Error / False = Error</returns>
        public bool Add_LoggingChannelGroup(string GroupName)
        {
            return (Add_LoggingChannelGroup(GroupName, oRootGroup));
        }

        /// <summary>
        /// Add a logging channel group to the group path given as argument
        /// </summary>
        /// <param name="GroupName">Name of the group to add</param>
        /// <param name="ParentGroupPath">Path of group in which the new group will be created</param>
        /// <returns>Group addition no error flag: True = No Error / False = Error</returns>
        public bool Add_LoggingChannelGroup(string GroupName, string ParentGroupPath)
        {
            LoggingChannelGroup oParentGroup = oRootGroup.Get_GroupAtPath(ParentGroupPath);

            if (!(oParentGroup == null))
            {
                return (Add_LoggingChannelGroup(GroupName, oParentGroup));
            }

            return (false);
        }

        /// <summary>
        /// Add a logging channel group to the group object reference given as argument
        /// </summary>
        /// <param name="GroupName">Name of the group to add</param>
        /// <param name="ParentGroup">Group object reference in which the new group will be created</param>
        /// <returns>Group addition no error flag: True = No Error / False = Error</returns>
        public bool Add_LoggingChannelGroup(string GroupName, LoggingChannelGroup ParentGroup)
        {
            if (!(GroupName.Equals("") || ParentGroup == null))
            {
                if (ParentGroup.Get_SubGroup(GroupName) == null)
                {
                    LoggingChannelGroup oNewGroup = new LoggingChannelGroup();
                    oNewGroup.Name = GroupName;
                    oNewGroup.FullPath = ParentGroup.FullPath + "\\" + GroupName;

                    ParentGroup.SubGroups.Add(oNewGroup);
                    return (true);
                }
            }

            return (false);
        }

        /// <summary>
        /// Returns the LoggingChannelGroup object from the logging channel group hierarchy corresponding to the name given as argument
        /// </summary>
        /// <param name="GroupName">Name of the group to retrieve</param>
        /// <returns>Group corresponding to the name given as argument</returns>
        /// /// <remarks>Returns null if the group is not found</remarks>
        public LoggingChannelGroup Get_LoggingChannelGroup(string GroupName)
        {
            return (oRootGroup.Get_TreeSubGroup(GroupName));
        }

        /// <summary>
        /// Delete the LoggingChannelGroup object at the path given as argument
        /// </summary>
        /// <param name="GroupPath">Path of the LoggingChannelGroup object to delete</param>
        /// <param name="GroupName">Name of the LoggingChannelGroup object to delete</param>
        /// <returns>Error flag: True: No error /False: Error</returns>
        public bool Delete_LoggingChannelGroup(string GroupPath,string GroupName)
        {
            if (!(GroupPath.Equals("") || GroupName.Equals("")))
            {
                LoggingChannelGroup oParentGroup = oRootGroup.Get_GroupAtPath(Get_ParentPath(GroupPath));

                if (!(oParentGroup == null))
                {
                    LoggingChannelGroup oGroup = oParentGroup.Get_SubGroup(GroupName);

                    if (!(oGroup == null))
                    {
                        oParentGroup.SubGroups.Remove(oGroup);
                        return (true);
                    }
                }
            }

            return (false);
        }

        #endregion

        #region CS_RecordLoggingConfiguration file management

        /// <summary>
        /// Write the current CS_RecordLoggingConfiguration into the default filepath
        /// </summary>
        /// <returns>No error flag:True = No Error / False = Error</returns>
        public bool Write_LoggingConfigurationFile()
        {
            if(!(LoggingConfigPath.Equals("")))
            {
                return (Write_LoggingConfigurationFile(LoggingConfigPath));
            }

            return (false);
        }

        /// <summary>
        /// Write the current CS_RecordLoggingConfiguration into the filepath given as argument
        /// </summary>
        /// <param name="FilePath">Path of the file to be written</param>
        /// <returns>No error flag:True = No Error / False = Error</returns>
        public bool Write_LoggingConfigurationFile(string FilePath)
        {
            if(!(FilePath.Equals("")))
            {
                XmlDocument oXDoc = new XmlDocument();

                XmlElement xConfig = oXDoc.CreateElement("RecordLoggingConfigruation");
                oXDoc.AppendChild(xConfig);

                XmlElement xRootGroup = oXDoc.CreateElement("LoggingChannels_RootGroup");
                xConfig.AppendChild(xRootGroup);

                List<string> SubGroupHierarchy = oRootGroup.Get_FullGroupsPathesList();
                foreach(string SubGroupPath in SubGroupHierarchy)
                {
                    XmlElement xGroup = oXDoc.CreateElement("GroupPath");

                    LoggingChannelGroup oSubGroup = oRootGroup.Get_GroupAtPath(SubGroupPath);
                    XmlAttribute xAtrGrpComment = oXDoc.CreateAttribute("Comment");
                    XmlAttribute xAtrGrpLogMode = oXDoc.CreateAttribute("GroupLoggingMode");
                    XmlAttribute xAtrGrpLogFreq = oXDoc.CreateAttribute("GroupLoggingFrequency");
                    XmlAttribute xAtrGrpDefLogFreq = oXDoc.CreateAttribute("GroupDefaultLoggingFrequency");

                    if (!(oSubGroup==null))
                    {
                        xAtrGrpComment.Value = oSubGroup.Comment;
                        xAtrGrpLogMode.Value = oSubGroup.GroupLoggingMode.ToString();
                        xAtrGrpLogFreq.Value = oSubGroup.GroupLoggingFrequency.ToString();
                        xAtrGrpDefLogFreq.Value = oSubGroup.GroupDefaultFrequency.ToString();

                    }
                    else
                    {
                        xAtrGrpComment.Value ="";
                        xAtrGrpLogMode.Value = ChannelLoggingMode.DefaultFrequency.ToString(); //Default value
                        xAtrGrpLogFreq.Value = "0"; //Default 
                        xAtrGrpDefLogFreq.Value = "0"; //Default value
                    }

                    xGroup.Attributes.Append(xAtrGrpComment);
                    xGroup.Attributes.Append(xAtrGrpLogMode);
                    xGroup.Attributes.Append(xAtrGrpLogFreq);
                    xGroup.Attributes.Append(xAtrGrpDefLogFreq);

                    xGroup.InnerText = SubGroupPath;
                    xRootGroup.AppendChild(xGroup);
                }

                XmlElement xChannels = oXDoc.CreateElement("LoggingChannels");
                xConfig.AppendChild(xChannels);

                List<LoggingChannelConfiguration> LoggingChannelHierarchy = oRootGroup.Get_FullChannelHierarchy();
                foreach(LoggingChannelConfiguration oChannel in LoggingChannelHierarchy)
                {
                    XmlElement xProp;

                    XmlElement xChan = oXDoc.CreateElement("ChannelConfiguration");

                    XmlAttribute xAtrName = oXDoc.CreateAttribute("Name");
                    xAtrName.Value = oChannel.Name;
                    xChan.Attributes.Append(xAtrName);

                    xProp = oXDoc.CreateElement("GroupPath");
                    xProp.InnerText = oChannel.Path;
                    xChan.AppendChild(xProp);

                    xProp = oXDoc.CreateElement("LoggingMode");
                    xProp.InnerText = oChannel.LoggingMode.ToString();
                    xChan.AppendChild(xProp);

                    xProp = oXDoc.CreateElement("LoggingFrequency");
                    xProp.InnerText = oChannel.LoggingFrequency.ToString();
                    xChan.AppendChild(xProp);

                    xProp = oXDoc.CreateElement("DefaultLoggingFrequency");
                    xProp.InnerText = oChannel.DefaultFrequency.ToString();
                    xChan.AppendChild(xProp);

                    xProp = oXDoc.CreateElement("Comment");
                    xProp.InnerText = oChannel.Comment;
                    xChan.AppendChild(xProp);

                    xChannels.AppendChild(xChan);
                }

                oXDoc.Save(FilePath);
                return (true);
            }

            return (false);
        }

        /// <summary>
        /// Read a CS_RecordLoggingConfiguration object into the specified file path
        /// </summary>
        /// <param name="FilePath">Path of the file to read</param>
        /// <returns>No error flag:True = No Error / False = Error</returns>
        public bool Read_LoggingConfigurationFile(string FilePath)
        {
            try
            {
                oRootGroup.LoggingChannels.Clear();
                oRootGroup.SubGroups.Clear();

                XmlDocument oXDoc = new XmlDocument();
                oXDoc.Load(FilePath);

                XmlNode xConfig = oXDoc.SelectSingleNode("RecordLoggingConfigruation");

                XmlNode xGroups = xConfig.SelectSingleNode("LoggingChannels_RootGroup");
                foreach(XmlNode xSubGroup in xGroups.ChildNodes)
                {
                    string[] GroupParents = xSubGroup.InnerText.Split('\\');

                    string GroupName = GroupParents.Last();
                    string ParentPath = "";

                    for (int i = 0; i < GroupParents.Length - 1; i++)
                    {
                        ParentPath += GroupParents[i];
                        if (i < GroupParents.Length - 2) ParentPath += "\\";
                    }

                    if(!(GroupName.Equals("") || ParentPath.Equals("")))
                    {
                        if (Add_LoggingChannelGroup(GroupName, ParentPath))
                        {
                            XmlAttribute xAtrComment = xSubGroup.Attributes["Comment"];
                            XmlAttribute xAtrGrpLogMode = xSubGroup.Attributes["GroupLoggingMode"];
                            XmlAttribute xAtrGrpLogFreq = xSubGroup.Attributes["GroupLoggingFrequency"];
                            XmlAttribute xAtrGrpDefLogFreq = xSubGroup.Attributes["GroupDefaultLoggingFrequency"];

                            LoggingChannelGroup oAddedGroup = oRootGroup.Get_GroupAtPath(xSubGroup.InnerText);

                            if (!(oAddedGroup == null))
                            {
                                if (!(xAtrComment == null))
                                {
                                    oAddedGroup.Comment = xAtrComment.Value;
                                }

                                if (!(xAtrGrpLogMode == null))
                                {
                                    oAddedGroup.GroupLoggingMode= (ChannelLoggingMode)(Enum.Parse(typeof(ChannelLoggingMode), xAtrGrpLogMode.Value));
                                }

                                if (!(xAtrGrpLogFreq == null))
                                {
                                    oAddedGroup.GroupLoggingFrequency = double.Parse(xAtrGrpLogFreq.Value);
                                }

                                if (!(xAtrGrpDefLogFreq == null))
                                {
                                    oAddedGroup.GroupDefaultFrequency = double.Parse(xAtrGrpDefLogFreq.Value);
                                }
                            }
                        }
                    }

                }

                XmlNode xChannels = xConfig.SelectSingleNode("LoggingChannels");
                foreach (XmlNode xChan in xChannels.ChildNodes)
                {
                    XmlNode xProp;

                    LoggingChannelConfiguration oChannel = new LoggingChannelConfiguration();

                    oChannel.Name = xChan.Attributes["Name"].Value;

                    xProp = xChan.SelectSingleNode("LoggingMode");
                    oChannel.LoggingMode = (ChannelLoggingMode)(Enum.Parse(typeof(ChannelLoggingMode), xProp.InnerText));

                    xProp = xChan.SelectSingleNode("LoggingFrequency");
                    oChannel.LoggingFrequency = double.Parse(xProp.InnerText);

                    xProp = xChan.SelectSingleNode("DefaultLoggingFrequency");
                    oChannel.DefaultFrequency = double.Parse(xProp.InnerText);

                    xProp = xChan.SelectSingleNode("Comment");
                    oChannel.Comment = xProp.InnerText;

                    xProp = xChan.SelectSingleNode("GroupPath");

                    if(!(xProp.InnerText.Equals("")))
                    {
                        Add_LoggingChannel(oChannel, xProp.InnerText);
                    }
                }

                return (true);
            }
            catch
            {
                return (false);
            }
        }

        #endregion

        #endregion
    }
}