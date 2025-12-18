using System;
using System.Collections.Generic;

namespace Portfolio.Services
{
    public class RepositoryDto
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Languages { get; set; } = new List<string>();
        public DateTimeOffset? LastCommit { get; set; }
        public int Stars { get; set; }
        public int PullRequests { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}
