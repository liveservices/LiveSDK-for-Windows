// ------------------------------------------------------------------------------
// Copyright (c) 2014 Microsoft Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
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