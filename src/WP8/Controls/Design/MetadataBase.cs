// ------------------------------------------------------------------------------
// Copyright 2014 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ------------------------------------------------------------------------------

namespace Microsoft.Live.Controls.Design
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.Windows.Design;
    using Microsoft.Windows.Design.Metadata;

    /// <summary>
    /// MetadataRegistration class.
    /// </summary>
    public class Metadata : IProvideAttributeTable
    {
        private static readonly string LiveServicesCategory = "LiveServices";

        private AttributeTable attributeTable;

        /// <summary>
        /// Construct a new instance of the class.
        /// </summary>
        public Metadata()
        {
        }

        /// <summary>
        /// Returns the design time metadata attribute table.
        /// </summary>
        public AttributeTable AttributeTable
        {
            get
            {
                if (this.attributeTable == null)
                {
                    this.attributeTable = BuildAttributeTable();
                }

                return this.attributeTable;
            }
        }

        /// <summary>
        /// Builds the design time attribute table.
        /// </summary>
        /// <returns>Custom attribute table.</returns>
        private static AttributeTable BuildAttributeTable()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            AddAttributes(builder);

            return builder.CreateTable();
        }

        /// <summary>
        /// Add design time attributes.
        /// </summary>
        /// <param name="builder">The assembly attribute table builder.</param>
        private static void AddAttributes(AttributeTableBuilder builder)
        {
            builder.AddCustomAttributes(
                typeof(SignInButton),
                new Attribute[] {
                    new DefaultPropertyAttribute("ClientId"),
                    new DefaultEventAttribute("SessionChanged"),
                    new ToolboxBrowsableAttribute(true),
                    new ToolboxCategoryAttribute(LiveServicesCategory),
                    new ToolboxTabNameAttribute(LiveServicesCategory)});

            EditorBrowsableAttribute browsableAlways = new EditorBrowsableAttribute(EditorBrowsableState.Always);
            CategoryAttribute categoryLive = new CategoryAttribute(LiveServicesCategory);

            DescriptionAttribute description = new DescriptionAttribute(StringResources.DescriptionBrandingType);
            builder.AddCustomAttributes(
                typeof(SignInButton),
                "Branding",
                new Attribute[] { browsableAlways, categoryLive, description });

            description = new DescriptionAttribute(StringResources.DescriptionClientId);
            builder.AddCustomAttributes(
                typeof(SignInButton),
                "ClientId",
                new Attribute[] { browsableAlways, categoryLive, description });

            description = new DescriptionAttribute(StringResources.DescriptionRedirectUri);
            builder.AddCustomAttributes(
                typeof(SignInButton),
                "RedirectUri",
                new Attribute[] { browsableAlways, categoryLive, description });

            description = new DescriptionAttribute(StringResources.DescriptionScopes);
            DefaultValueAttribute defaultValue = new DefaultValueAttribute("wl.signin");
            builder.AddCustomAttributes(
                typeof(SignInButton),
                "Scopes",
                new Attribute[] { browsableAlways, categoryLive, description, defaultValue });

            description = new DescriptionAttribute(StringResources.DescriptionTextType);
            builder.AddCustomAttributes(
                typeof(SignInButton),
                "TextType",
                new Attribute[] { browsableAlways, categoryLive, description });

            description = new DescriptionAttribute(StringResources.DescriptionSigninText);
            builder.AddCustomAttributes(
                typeof(SignInButton),
                "SignInText",
                new Attribute[] { browsableAlways, categoryLive, description });

            description = new DescriptionAttribute(StringResources.DescriptionSignoutText);
            builder.AddCustomAttributes(
                typeof(SignInButton),
                "SignOutText",
                new Attribute[] { browsableAlways, categoryLive, description });            
        }
    }
}