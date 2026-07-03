using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tawala.TextEditor
{
    public partial class TabDialog : Form
    {
        private double[] tabsInInches = new double[0];
        private bool updateRequired = false;
        private int maxTabs = 0;

        public TabDialog(int maxTabs)
        {
            this.maxTabs = maxTabs;
            InitializeComponent();
        }

        public double[] TabsInInches
        {
            get
            {
                return tabsInInches;
            }
            set
            {
                tabsInInches = value;
                for (int i = 0; i < tabsInInches.Length; ++i)
                {
                    listBoxTabs.Items.Add(tabsInInches[i]);
                }
                syncButtonStates();
            }
        }

        private void processListBox()
        {
            tabsInInches = new double[listBoxTabs.Items.Count];

            for (int i = 0; i < tabsInInches.Length; ++i)
            {
                tabsInInches[i] = (double)(listBoxTabs.Items[i]);
            }
        }

        private void syncButtonStates()
        {
            buttonSet.Enabled = listBoxTabs.Items.Count <= maxTabs;
            buttonClear.Enabled = listBoxTabs.SelectedIndices.Count > 0;
            buttonClearAll.Enabled = listBoxTabs.Items.Count > 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (updateRequired)
            {
                processListBox();
            }
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            double tab = Convert.ToDouble(maskedTextBoxTab.Text);
            if (tab > 0.0 && tab <= 6.5)
            {
                if (!listBoxTabs.Items.Contains(tab))
                {
                    updateRequired = true;
                    listBoxTabs.Items.Add(tab);
                }
            }

            syncButtonStates();
            maskedTextBoxTab.Focus();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            while (listBoxTabs.SelectedIndices.Count > 0)
            {
                updateRequired = true;
                listBoxTabs.Items.RemoveAt(listBoxTabs.SelectedIndices[listBoxTabs.SelectedIndices.Count - 1]);
            }
            syncButtonStates();
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            updateRequired = true;
            listBoxTabs.Items.Clear();
            syncButtonStates();
        }

        private void listBoxTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            syncButtonStates();
        }
    }
}