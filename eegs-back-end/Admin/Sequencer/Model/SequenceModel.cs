using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.Sequencer.Model
{
    public class SequenceModel
    {
        public List<Sequence> seq_list { get; set; }
    }

    public class Sequence
    {
        public string domain_id { get; set; }
        public string form_id { get; set; }
        public int order { get; set; }
        public string user_id { get; set; }
    }
}
