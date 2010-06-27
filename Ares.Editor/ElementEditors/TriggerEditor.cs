using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ares.ModelInfo;

namespace Ares.Editor.ElementEditors
{
    partial class TriggerEditor : EditorBase
    {
        public TriggerEditor()
        {
            InitializeComponent();
        }

        private Ares.Data.IModeElement m_Element;

        public void SetElement(Ares.Data.IModeElement element)
        {
            m_Element = element;
            ElementId = element.Id;
            UpdateTitle();
            UpdateData();
            Actions.ElementChanges.Instance.AddListener(-1, Update);
            Actions.ElementChanges.Instance.AddListener(element.Id, Update);
        }

        private void Update(int elementID, Actions.ElementChanges.ChangeType changeType)
        {
            if (elementID != ElementId)
            {
                if (changeType == Actions.ElementChanges.ChangeType.Changed)
                {
                    UpdateTriggerDesc();
                }
                else if (changeType == Actions.ElementChanges.ChangeType.TriggerChanged)
                {
                    UpdateErrorProvider();
                }
            }
            else
            {
                if (changeType == Actions.ElementChanges.ChangeType.Renamed)
                    UpdateTitle();
                else if (changeType == Actions.ElementChanges.ChangeType.TriggerChanged)
                    UpdateData();
                else if (changeType == Actions.ElementChanges.ChangeType.Removed)
                    Close();
            }
        }

        private void UpdateTitle()
        {
            this.Text = String.Format(StringResources.TriggerEditorTitle, m_Element.Title);
        }

        private void UpdateData()
        {
            listen = false;

            Ares.Data.ITrigger trigger = m_Element.Trigger;
            if (trigger != null)
            {
                if (trigger.TriggerType == Data.TriggerType.Key)
                {
                    Ares.Data.IKeyTrigger keyTrigger = trigger as Ares.Data.IKeyTrigger;
                    KeysConverter converter = new KeysConverter();
                    selectKeyButton.Text = converter.ConvertToString((Keys)keyTrigger.KeyCode);
                }
                else
                {
                    selectKeyButton.Text = StringResources.NoKey;
                }
                stopMusicBox.Checked = trigger.StopMusic;
                stopSoundsBox.Checked = trigger.StopSounds;
            }
            else
            {
                selectKeyButton.Text = StringResources.NoKey;
                stopMusicBox.Checked = false;
                stopSoundsBox.Checked = false;
            }
            UpdateTriggerDesc();
            UpdateErrorProvider();

            listen = true;
        }

        private void UpdateErrorProvider()
        {
            int keyCode = GetCurrentKeyCode();
            if (keyCode == -1)
            {
                errorProvider.SetError(selectKeyButton, String.Empty);
            }
            else
            {
                errorProvider.SetError(selectKeyButton, ModelChecks.Instance.GetErrorForKey(m_Element, keyCode));
            }
        }

        private int GetCurrentKeyCode()
        {
            if (selectKeyButton.Text == StringResources.NoKey)
            {
                return -1;
            }
            else
            {
                KeysConverter converter = new KeysConverter();
                return (int)(Keys)converter.ConvertFromString(selectKeyButton.Text);
            }
        }

        private void UpdateTriggerDesc()
        {
            if (m_Element.IsEndless())
            {
                triggerDescLabel.Text = StringResources.ElementIsEndless;
            }
            else
            {
                triggerDescLabel.Text = StringResources.ElementIsSingle;
            }
            if (m_Element.AlwaysStartsMusic())
            {
                stopMusicBox.Checked = true;
                stopMusicBox.Enabled = false;
                stopMusicLabel.Visible = true;
            }
            else
            {
                stopMusicBox.Enabled = true;
                stopMusicLabel.Visible = false;
            }
            listen = true;
        }

        private void Commit()
        {
            if (selectKeyButton.Text != StringResources.NoKey)
            {
                Ares.Data.IKeyTrigger trigger = Ares.Data.DataModule.ElementFactory.CreateKeyTrigger();
                trigger.KeyCode = GetCurrentKeyCode();
                trigger.StopMusic = stopMusicBox.Checked;
                trigger.StopSounds = stopSoundsBox.Checked;
                Ares.Editor.Actions.Actions.Instance.AddNew(new Ares.Editor.Actions.SetModeElementTriggerAction(m_Element, trigger));
            }
            else
            {
                Ares.Data.ITrigger trigger = Ares.Data.DataModule.ElementFactory.CreateNoTrigger();
                trigger.StopMusic = stopMusicBox.Checked;
                trigger.StopSounds = stopSoundsBox.Checked;
                Ares.Editor.Actions.Actions.Instance.AddNew(new Ares.Editor.Actions.SetModeElementTriggerAction(m_Element, trigger));
            }
        }

        private void stopSoundsBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!listen)
                return;
            Commit();
        }

        private void SelectKey()
        {
            int keyCode = 0;
            DialogResult result = Dialogs.KeyDialog.Show(this, out keyCode);
            if (result != DialogResult.Cancel)
            {
                KeysConverter converter = new KeysConverter();
                Keys key = (Keys)keyCode;
                selectKeyButton.Text = converter.ConvertToString(key);
                UpdateErrorProvider();
                Commit();
            }
        }

        private bool listen = true;

        private void stopMusicBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!listen)
                return;
            Commit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectKey();
        }


    }
}
