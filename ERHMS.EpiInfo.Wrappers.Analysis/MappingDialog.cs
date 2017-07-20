using Epi.Windows.Analysis.Dialogs;
using Epi.Windows.Analysis.Forms;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
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
            DialogResult = DialogResult.OK;
        }

        private new void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
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
                string suggestedTarget = targets.FirstOrDefault(target => target.EqualsIgnoreCase(source));
                dgvMappings.Rows.Add(source, suggestedTarget ?? EmptyTarget);
            }
        }

        public MappingCollection GetMappings()
        {
            MappingCollection mappings = new MappingCollection();
            foreach (DataGridViewRow row in dgvMappings.Rows)
            {
                string source = (string)row.Cells[nameof(colSource)].Value;
                string target = (string)row.Cells[nameof(colTarget)].Value;
                if (target.EqualsIgnoreCase(EmptyTarget))
                {
                    continue;
                }
                mappings.Add(source, target);
            }
            return mappings;
        }
    }
}
