using Epi.Windows.Analysis.Dialogs;
using Epi.Windows.Analysis.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.Wrappers
{
    internal partial class MappingDialog : CommandDesignDialog
    {
        public const string EmptyTarget = "";

        private ICollection<string> sources;
        private ICollection<string> targets;

        public MappingDialog(AnalysisMainForm form, IEnumerable<string> sources, IEnumerable<string> targets)
            : base(form)
        {
            InitializeComponent();
            this.sources = sources.ToList();
            this.targets = targets.ToList();
            BindMappings();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (Validate())
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void BindMappings()
        {
            colTarget.Items.Clear();
            colTarget.Items.Add(EmptyTarget);
            foreach (string target in targets)
            {
                colTarget.Items.Add(target);
            }
            foreach (string source in sources)
            {
                string suggestedTarget = targets.FirstOrDefault(target => target.Equals(source, StringComparison.OrdinalIgnoreCase));
                dgvMappings.Rows.Add(source, suggestedTarget ?? EmptyTarget);
            }
        }

        private IEnumerable<Mapping> GetMappingsInternal()
        {
            foreach (DataGridViewRow row in dgvMappings.Rows)
            {
                string source = (string)row.Cells[nameof(colSource)].Value;
                string target = (string)row.Cells[nameof(colTarget)].Value;
                if (target != null && !target.Equals(EmptyTarget, StringComparison.OrdinalIgnoreCase))
                {
                    yield return new Mapping(source, target);
                }
            }
        }

        private new bool Validate()
        {
            IDictionary<string, int> counts = new Dictionary<string, int>();
            foreach (Mapping mapping in GetMappingsInternal())
            {
                int count;
                counts.TryGetValue(mapping.Target, out count);
                counts[mapping.Target] = ++count;
            }
            ICollection<string> duplicates = counts.Where(count => count.Value > 1)
                .Select(count => count.Key)
                .ToList();
            if (duplicates.Count > 0)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine("The following destination fields have more than one source:");
                message.AppendLine();
                foreach (string duplicate in duplicates.OrderBy(duplicate => duplicate))
                {
                    message.AppendLine(duplicate);
                }
                MessageBox.Show(message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public MappingCollection GetMappings()
        {
            MappingCollection mappings = new MappingCollection();
            mappings.AddRange(GetMappingsInternal());
            return mappings;
        }
    }
}
