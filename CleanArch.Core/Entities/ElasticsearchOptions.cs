﻿namespace CleanArch.Core.Entities
{
    public class ElasticsearchOptions
    {
        public string Uri { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DefaultIndex { get; set; }
    }
}
