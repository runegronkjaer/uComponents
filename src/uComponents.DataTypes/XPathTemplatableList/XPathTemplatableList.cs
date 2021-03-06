﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using umbraco;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.member;
using umbraco.NodeFactory;

namespace uComponents.DataTypes.XPathTemplatableList
{
    /// <summary>
    /// Strongly typed obj that's returned from uQuery .GetProperty{XPathCheckBoxList}("alias");
    /// </summary>
    public class XPathTemplatableList : uQuery.IGetProperty
    {
		/// <summary>
		/// Gets the selected nodes.
		/// </summary>
		/// <value>
		/// The selected nodes.
		/// </value>
        public IEnumerable<Node> SelectedNodes { get; private set; }

		/// <summary>
		/// Gets the selected media.
		/// </summary>
		/// <value>
		/// The selected media.
		/// </value>
        public IEnumerable<Media> SelectedMedia { get; private set; }

		/// <summary>
		/// Gets the selected members.
		/// </summary>
		/// <value>
		/// The selected members.
		/// </value>
        public IEnumerable<Member> SelectedMembers { get; private set; }

        void uQuery.IGetProperty.LoadPropertyValue(string value)
        {
            this.SelectedNodes = new Node[] { };
            this.SelectedMedia = new Media[] { };
            this.SelectedMembers = new Member[] { };

            if (!string.IsNullOrWhiteSpace(value))
            {
                /*
                    <XPathTemplatableList Type="c66ba18e-eaf3-4cff-8a22-41b16d66a972">
                        <Item Value="1" />
                        <Item Value="9" />
                    </XPathTemplatableList>
                */

                XDocument valueXDocument = this.GetXDocument(value);
                if (valueXDocument != null)
                {
                    IEnumerable<int> values = valueXDocument.Descendants("Item").Attributes("Value").Select(x => int.Parse(x.Value));

                    Guid typeGuid;
                    if (Guid.TryParse(valueXDocument.Root.Attribute("Type").Value, out typeGuid))
                    {
                        switch (uQuery.GetUmbracoObjectType(typeGuid))
                        {
                            case uQuery.UmbracoObjectType.Document:
                                this.SelectedNodes = values.Select(uQuery.GetNode);
                                break;

                            case uQuery.UmbracoObjectType.Media:
                                this.SelectedMedia = values.Select(uQuery.GetMedia);
                                break;

                            case uQuery.UmbracoObjectType.Member:
                                this.SelectedMembers = values.Select(uQuery.GetMember);
                                break;
                        }
                    }
                }
            }
        }

        private XDocument GetXDocument(string value)
        {
            try
            {
                return XDocument.Parse(value);
            }
            catch
            {
                // invalid xml
                return null;
            }
        }


        //int[] uQuery.IPickerRelations.GetIds(string value)
        //{
        //    /*
        //        <XPathTemplatableList Type="c66ba18e-eaf3-4cff-8a22-41b16d66a972">
        //            <Item Value="1" />
        //            <Item Value="9" />
        //        </XPathTemplatableList>
        //    */

        //    XDocument valueXDocument = XDocument.Load(value);
            
        //    return valueXDocument.Descendants("Item").Attributes("Value").Select(x => int.Parse(x.Value));
        //}
    }
}
