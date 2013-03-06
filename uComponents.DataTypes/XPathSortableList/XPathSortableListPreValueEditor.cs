﻿using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.editorControls;
using umbraco;
using System.Configuration;
using uComponents.DataTypes.Shared.Extensions;
using System.Web;

namespace uComponents.DataTypes.XPathSortableList
{
    using System.Linq;

    using umbraco.macroRenderings;

    /// <summary>
    /// Prevalue Editor for XPath AutoComplete
    /// </summary>
    public class XPathSortableListPreValueEditor : uComponents.DataTypes.Shared.PrevalueEditors.AbstractJsonPrevalueEditor
    {
        /// <summary>
        /// Radio buttons to select type of node to pick from: Content / Media / Members
        /// </summary>
        private RadioButtonList typeRadioButtonList = new RadioButtonList();

        /// <summary>
        /// TextBox control to get the XPath expression
        /// </summary>
        private TextBox xPathTextBox = new TextBox();

        /// <summary>
        /// RequiredFieldValidator to ensure an XPath expression has been entered
        /// </summary>
        private RequiredFieldValidator xPathRequiredFieldValidator = new RequiredFieldValidator();

        /// <summary>
        /// Server side validation of XPath expression
        /// </summary>
        private CustomValidator xPathCustomValidator = new CustomValidator();

        /// <summary>
        /// Use an optional thumbnail from a property alias
        /// </summary>
        private propertyTypePicker thumbnailPropertyDropDown = new propertyTypePicker();

        /// <summary>
        /// Min number of items that must be selected - defaults to 0
        /// </summary>
        private TextBox minItemsTextBox = new TextBox();

        /// <summary>
        /// Custom validator to ensure that if relevant, then min items is less than or equal to max items
        /// </summary>
        private CustomValidator minItemsCustomValidator = new CustomValidator();

        /// <summary>
        /// Max number of items that can be selected - defaults to 0 (anything that's not a +ve integer imposes no limit)
        /// </summary>
        private TextBox maxItemsTextBox = new TextBox();

        /// <summary>
        /// If max items is set (ie not 0) then it must be greater than min items
        /// </summary>
        private CustomValidator maxItemsCustomValidator = new CustomValidator();

        /// <summary>
        /// if enabled then the same item can be seleted multiple times
        /// </summary>
        private CheckBox allowDuplicatesCheckBox = new CheckBox();

        /// <summary>
        /// Data object used to define the configuration status of this PreValueEditor
        /// </summary>
        private XPathSortableListOptions options = null;

        /// <summary>
        /// Initialize a new instance of XPathSortableListPreValueEditor
        /// </summary>
        /// <param name="dataType">XPathSortableListDataType</param>
        public XPathSortableListPreValueEditor(umbraco.cms.businesslogic.datatype.BaseDataType dataType)
            : base(dataType, umbraco.cms.businesslogic.datatype.DBTypes.Ntext)
        {
        }

        /// <summary>
        /// Gets the options data object that represents the current state of this datatypes configuration
        /// </summary>
        internal XPathSortableListOptions Options
        {
            get
            {
                if (this.options == null)
                {
                    // Deserialize any stored settings for this PreValueEditor instance
                    this.options = this.GetPreValueOptions<XPathSortableListOptions>();

                    // If still null, ie, object couldn't be de-serialized from PreValue[0] string value
                    if (this.options == null)
                    {
                        // Create a new Options data object with the default values
                        this.options = new XPathSortableListOptions();
                    }
                }

                return this.options;
            }
        }

        /// <summary>
        /// Creates all of the controls and assigns all of their properties
        /// </summary>
        protected override void CreateChildControls()
        {
            //radio buttons to select type of nodes that can be picked (Document, Media or Member)
            this.typeRadioButtonList.Items.Add(new ListItem(uQuery.UmbracoObjectType.Document.GetFriendlyName(), uQuery.UmbracoObjectType.Document.GetGuid().ToString()));
            this.typeRadioButtonList.Items.Add(new ListItem(uQuery.UmbracoObjectType.Media.GetFriendlyName(), uQuery.UmbracoObjectType.Media.GetGuid().ToString()));
            this.typeRadioButtonList.Items.Add(new ListItem(uQuery.UmbracoObjectType.Member.GetFriendlyName(), uQuery.UmbracoObjectType.Member.GetGuid().ToString()));

            this.xPathTextBox.ID = "xPathTextBox";
            this.xPathTextBox.CssClass = "umbEditorTextField";

            this.xPathRequiredFieldValidator.ControlToValidate = this.xPathTextBox.ID;
            this.xPathRequiredFieldValidator.Display = ValidatorDisplay.Dynamic;
            this.xPathRequiredFieldValidator.ErrorMessage = " XPath expression required";

            this.xPathCustomValidator.ControlToValidate = this.xPathTextBox.ID;
            this.xPathCustomValidator.Display = ValidatorDisplay.Dynamic;
            this.xPathCustomValidator.ServerValidate += this.XPathCustomValidator_ServerValidate;

            this.thumbnailPropertyDropDown.ID = "thumbnailPropertyDropDown";

            this.minItemsTextBox.ID = "minSelectionItemsTextBox";
            this.minItemsTextBox.Width = 30;
            this.minItemsTextBox.MaxLength = 2;
            this.minItemsTextBox.AutoCompleteType = AutoCompleteType.None;

            this.minItemsCustomValidator.ControlToValidate = this.minItemsTextBox.ID;
            this.minItemsCustomValidator.Display = ValidatorDisplay.Dynamic;
            this.minItemsCustomValidator.ServerValidate += this.MinItemsCustomValidatorServerValidate;

            this.maxItemsTextBox.ID = "maxSelectionItemsTextBox";
            this.maxItemsTextBox.Width = 30;
            this.maxItemsTextBox.MaxLength = 2;
            this.maxItemsTextBox.AutoCompleteType = AutoCompleteType.None;

            this.maxItemsCustomValidator.ControlToValidate = this.maxItemsTextBox.ID;
            this.maxItemsCustomValidator.Display = ValidatorDisplay.Dynamic;
            this.maxItemsCustomValidator.ServerValidate += this.MaxItemsCustomValidator_ServerValidate;

            this.allowDuplicatesCheckBox.ID = "allowDuplicatesCheckBox";

            this.Controls.AddPrevalueControls(
                this.typeRadioButtonList,
                this.xPathTextBox,
                this.xPathRequiredFieldValidator,
                this.xPathCustomValidator,
                this.thumbnailPropertyDropDown,
                this.minItemsTextBox,
                this.minItemsCustomValidator,
                this.maxItemsTextBox,
                this.maxItemsCustomValidator,
                this.allowDuplicatesCheckBox);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.Page.IsPostBack)
            {
                this.typeRadioButtonList.SelectedValue = this.Options.Type;
                this.xPathTextBox.Text = this.Options.XPath;

                if (this.thumbnailPropertyDropDown.Items.Contains(new ListItem(this.Options.ThumbnailProperty)))
                {
                    this.thumbnailPropertyDropDown.SelectedValue = this.Options.ThumbnailProperty;
                }

                this.minItemsTextBox.Text = this.Options.MinItems.ToString();
                this.maxItemsTextBox.Text = this.Options.MaxItems.ToString();
                this.allowDuplicatesCheckBox.Checked = this.Options.AllowDuplicates;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">xPathCustomValidator</param>
        /// <param name="args"></param>
        private void XPathCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string xpath = args.Value;
            bool isValid = false;

            uQuery.UmbracoObjectType umbracoObjectType = uQuery.GetUmbracoObjectType(new Guid(this.typeRadioButtonList.SelectedValue));

            try
            {
                switch (umbracoObjectType)
                {
                    case uQuery.UmbracoObjectType.Document: uQuery.GetNodesByXPath(xpath); 
                        break;
                    case uQuery.UmbracoObjectType.Media: uQuery.GetMediaByXPath(xpath);
                        break;
                    case uQuery.UmbracoObjectType.Member: uQuery.GetMembersByXPath(xpath);
                        break;
                }

                isValid = true;
            }
            catch
            {
                this.xPathCustomValidator.ErrorMessage = " Error in XPath expression";
            }

            args.IsValid = isValid;
        }

        private void MinItemsCustomValidatorServerValidate(object source, ServerValidateEventArgs args)
        {            
            bool isValid = false;

            int minItems;
            int maxItems;

            if (int.TryParse(args.Value, out minItems))
            {
                if (minItems >= 0)
                {
                    if (int.TryParse(this.maxItemsTextBox.Text, out maxItems))
                    {
                        if (minItems <= maxItems || maxItems == 0)
                        {
                            isValid = true;
                        }
                        else
                        {
                            this.minItemsCustomValidator.ErrorMessage = " Min Items must be less than or equal to Max Items";            
                        }
                    }
                }
                else
                {
                    this.minItemsCustomValidator.ErrorMessage = " Min Items must be 0 or greater";
                }
            }
            else
            {
                this.minItemsCustomValidator.ErrorMessage = " Min Items must be a number";
            }

            args.IsValid = isValid;
        }
        
        private void MaxItemsCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            bool isValid = false;

            int minItems;
            int maxItems;

            if (int.TryParse(args.Value, out maxItems))
            {
                if (maxItems >= 0)
                {
                    if (int.TryParse(this.minItemsTextBox.Text, out minItems))
                    {
                        if (maxItems >= minItems || maxItems == 0)
                        {
                            isValid = true;
                        }
                        else
                        {
                            this.maxItemsCustomValidator.ErrorMessage = " Max Items must be greater than or equal to Min Items";
                        }
                    }
                }
                else
                {
                    this.maxItemsCustomValidator.ErrorMessage = " Max Items must be 0 or greater";
                }
            }
            else
            {
                this.maxItemsCustomValidator.ErrorMessage = " Max Items must be a number";
            }

            args.IsValid = isValid;
        }


        /// <summary>
        /// Saves the pre value data to Umbraco
        /// </summary>
        public override void Save()
        {
            if (this.Page.IsValid)
            {
                this.Options.Type = this.typeRadioButtonList.SelectedValue;
                this.Options.XPath = this.xPathTextBox.Text;
                this.Options.ThumbnailProperty = this.thumbnailPropertyDropDown.SelectedValue;

                // ensure min and max items are valid numbers
                int minItems;
                int.TryParse(this.minItemsTextBox.Text, out minItems);
                this.Options.MinItems = minItems;

                int maxItems;
                int.TryParse(this.maxItemsTextBox.Text, out maxItems);
                this.Options.MaxItems = maxItems;

                this.Options.AllowDuplicates = this.allowDuplicatesCheckBox.Checked;

                this.SaveAsJson(this.Options);  // Serialize to Umbraco database field
            }
        }

        /// <summary>
        /// Replaces the base class writer and instead uses the shared uComponents extension method, to inject consistant markup
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddPrevalueRow("Type", @"the xml schema to query", this.typeRadioButtonList);
            writer.AddPrevalueRow("XPath Expression", @"expects a result set of node, meda or member elements", this.xPathTextBox, this.xPathRequiredFieldValidator, this.xPathCustomValidator);
            writer.AddPrevalueRow("Thumbnail Property", "expects a string url (todo: fallback to media id/umbracoFile)", this.thumbnailPropertyDropDown);
            writer.AddPrevalueRow("Min Items", "number of items that must be selected", this.minItemsTextBox, this.minItemsCustomValidator);
            writer.AddPrevalueRow("Max Items", "number of items that can be selected - 0 means no limit", this.maxItemsTextBox, this.maxItemsCustomValidator);
            writer.AddPrevalueRow("Allow Duplicates", "when checked, duplicate values can be selected", this.allowDuplicatesCheckBox);
        }
    }
}
