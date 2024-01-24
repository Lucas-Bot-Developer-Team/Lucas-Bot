﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EleCho.GoCqHttpSdk.Action.Model.ResultData
{
    internal class CqGetVersionInformationActionResultDataModel : CqActionResultDataModel
    {
        [JsonConstructor]
        public CqGetVersionInformationActionResultDataModel(string app_name, string app_version, string app_full_name, string protocol_version, string coolq_edition, string coolq_directory, bool go_cqhttp, string plugin_version, int plugin_build_number, string plugin_build_configuration, string runtime_version, string runtime_os, string version, int protocol_name)
        {
            this.app_name = app_name;
            this.app_version = app_version;
            this.app_full_name = app_full_name;
            this.protocol_name = protocol_name;
            this.protocol_version = protocol_version;
            this.coolq_edition = coolq_edition;
            this.coolq_directory = coolq_directory;
            this.go_cqhttp = go_cqhttp;
            this.plugin_version = plugin_version;
            this.plugin_build_number = plugin_build_number;
            this.plugin_build_configuration = plugin_build_configuration;
            this.runtime_version = runtime_version;
            this.runtime_os = runtime_os;
            this.version = version;
        }

        public string app_name { get; }
        public string app_version { get; }
        public string app_full_name { get; }
        public int protocol_name { get; }
        public string protocol_version { get; }
        public string coolq_edition { get; }
        public string coolq_directory { get; }

        [JsonPropertyName("go-cqhttp")]
        public bool go_cqhttp { get; }

        public string plugin_version { get; }
        public int plugin_build_number { get; }
        public string plugin_build_configuration { get; }

        public string runtime_version { get; }
        public string runtime_os { get; }

        public string version { get; }
    }
}
