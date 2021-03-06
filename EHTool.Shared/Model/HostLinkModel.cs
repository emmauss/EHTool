﻿using EHTool.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Text;

namespace EHTool.Shared.Model
{
    [DataContract]
    public abstract class HostLinkModel
    {
        [IgnoreDataMember]
        private const string EHentaiLink = "http://g.e-hentai.org/";
        [IgnoreDataMember]
        private const string ExHentaiLink = "http://exhentai.org/";
        [DataMember]
        public ServerTypes ServerType { get; internal set; }
        [IgnoreDataMember]
        protected bool NetworkAvailable => NetworkInterface.GetIsNetworkAvailable();
        [IgnoreDataMember]
        protected string HostLink =>
            ServerType == ServerTypes.EHentai ? EHentaiLink : ExHentaiLink;
    }
}
