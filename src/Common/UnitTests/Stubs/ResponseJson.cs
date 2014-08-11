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

namespace Microsoft.Live.UnitTest
{
    using System;

    internal static class ResponseJson
    {
        public static readonly string Me =
          @"{
               ""id"": ""9cbbca7a47c23a9b"", 
               ""name"": ""Jack Jill"", 
               ""first_name"": ""Jack"", 
               ""last_name"": ""Jill"", 
               ""link"": ""http://profile.live-int.com/cid-9cbbca7a47c23a9b/"", 
               ""gender"": null, 
               ""locale"": ""en_US"", 
               ""updated_time"": ""2011-03-03T23:28:49+0000""
            }";

        public static readonly string Contacts =
            @"{
               ""data"": [
                  {
                     ""id"": ""contact.e27a6c43000000000000000000000000"", 
                     ""first_name"": ""Shelly - Hotmail1"", 
                     ""last_name"": ""Guo"", 
                     ""name"": ""Shelly - Hotmail1 Guo"", 
                     ""gender"": null, 
                     ""is_friend"": false, 
                     ""is_favorite"": false, 
                     ""user_id"": ""a11c62c1e566c681"", 
                     ""type"": ""contact"", 
                     ""email_hashes"": [
                        ""ac77e642c166d7f50dd1a69e94236cc72445dec0216171e11c3d5dd7e8e45f72""
                     ], 
                     ""updated_time"": ""2011-08-03T14:31:16+0000""
                  }, {
                     ""id"": ""contact.1e7d626c000000000000000000000000"", 
                     ""first_name"": ""Brandon"", 
                     ""last_name"": ""Smith"", 
                     ""name"": ""Brandon Smith"", 
                     ""gender"": null, 
                     ""is_friend"": false, 
                     ""is_favorite"": false, 
                     ""user_id"": null, 
                     ""type"": ""contact"", 
                     ""email_hashes"": [
                        ""04a8b9db1327d6e47878609e803d700230a14e52e37ed053d31b5db68ebce7f8"", 
                        ""96ffd40fa4bda5ba185705ba45442e46069bffbb9b15b02304890e9b149c23ad""
                     ], 
                     ""updated_time"": ""2011-08-03T14:29:06+0000""
                  }, {
                     ""id"": ""contact.5ac596ec000000000000000000000000"", 
                     ""first_name"": ""Shelly - Live 1"", 
                     ""last_name"": ""Guo"", 
                     ""name"": ""Shelly - Live 1 Guo"", 
                     ""gender"": null, 
                     ""is_friend"": false, 
                     ""is_favorite"": true, 
                     ""user_id"": ""ff6da21daa0ab1bf"", 
                     ""type"": ""contact"", 
                     ""email_hashes"": [
                        ""9a4375e90142d18ecb4d71e86b56b21ba71ece6bba34b7db97dbe4ad18ed2c32""
                     ], 
                     ""updated_time"": ""2011-08-03T14:31:33+0000""
                  }
               ]
            }";

        public static readonly string Contact =
            @"{
                ""id"": ""contact.1e7d626c000000000000000000000000"", 
                ""first_name"": ""Brandon"", 
                ""last_name"": ""Smith"", 
                ""name"": ""Brandon Smith"", 
                ""gender"": null, 
                ""is_friend"": false, 
                ""is_favorite"": false, 
                ""user_id"": null, 
                ""type"": ""contact"", 
                ""email_hashes"": [
                    ""04a8b9db1327d6e47878609e803d700230a14e52e37ed053d31b5db68ebce7f8"", 
                    ""96ffd40fa4bda5ba185705ba45442e46069bffbb9b15b02304890e9b149c23ad""
                ], 
                ""updated_time"": ""2011-08-03T14:29:06+0000""
            }";

        public static readonly string File =
            @"{
                 ""id"": ""file.0bee3874468ef5b6.BEE3874468EF5B6!759"", 
                 ""from"": {
                    ""name"": ""Jack Jill"", 
                    ""id"": ""0bee3874468ef5b6""
                 }, 
                 ""name"": ""PhotoChooser中文名.jpg"", 
                 ""description"": null, 
                 ""parent_id"": ""folder.0bee3874468ef5b6"", 
                 ""size"": 183028, 
                 ""comments_count"": 0, 
                 ""comments_enabled"": false, 
                 ""tags_count"": 0, 
                 ""tags_enabled"": true, 
                 ""is_embeddable"": true, 
                 ""picture"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg:Thumbnail/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg"", 
                 ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg:Default,Largest/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg"", 
                 ""upload_location"": ""https://beta.apis.live.net/v5.0/file.0bee3874468ef5b6.BEE3874468EF5B6!759/content/"", 
                 ""images"": [
                    {
                       ""height"": 600, 
                       ""width"": 800, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg:Scaled800,WebReady/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg"", 
                       ""type"": ""normal""
                    }, {
                       ""height"": 132, 
                       ""width"": 176, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg:MobileReady/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg"", 
                       ""type"": ""album""
                    }, {
                       ""height"": 72, 
                       ""width"": 96, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg:Thumbnail/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg"", 
                       ""type"": ""thumbnail""
                    }, {
                       ""height"": 600, 
                       ""width"": 800, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg:Default,Largest/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg"", 
                       ""type"": ""full""
                    }
                 ], 
                 ""link"": ""https://skydrive.live.com/redir.aspx?cid\u003d0bee3874468ef5b6\u0026page\u003dview\u0026resid\u003dBEE3874468EF5B6!759\u0026parid\u003dBEE3874468EF5B6!103"", 
                 ""when_taken"": ""2010-07-19T03:39:24+0000"", 
                 ""height"": 600, 
                 ""width"": 800, 
                 ""type"": ""photo"", 
                 ""location"": null, 
                 ""shared_with"": {
                    ""access"": ""Just me""
                 }, 
                 ""created_time"": ""2012-02-29T06:28:23+0000"", 
                 ""updated_time"": ""2012-02-29T06:28:23+0000""
              }";

        public static readonly string Files =
            @"{
           ""data"": [
              {
                 ""id"": ""folder.0bee3874468ef5b6.BEE3874468EF5B6!521"", 
                 ""from"": {
                    ""name"": ""Jack Jill"", 
                    ""id"": ""0bee3874468ef5b6""
                 }, 
                 ""name"": ""Mobile Uploads"", 
                 ""description"": null, 
                 ""parent_id"": ""folder.0bee3874468ef5b6"", 
                 ""upload_location"": ""https://apis.live.net/v5.0/folder.0bee3874468ef5b6.BEE3874468EF5B6!521/files/"", 
                 ""is_embeddable"": true, 
                 ""count"": 0, 
                 ""link"": ""https://skydrive.live.com/redir.aspx?cid\u003d0bee3874468ef5b6\u0026page\u003dview\u0026resid\u003dBEE3874468EF5B6!521\u0026parid\u003dBEE3874468EF5B6!103"", 
                 ""type"": ""folder"", 
                 ""shared_with"": {
                    ""access"": ""Just me""
                 }, 
                 ""created_time"": ""2011-10-20T05:04:25+0000"", 
                 ""updated_time"": ""2011-10-20T05:04:25+0000""
              }, {
                 ""id"": ""folder.0bee3874468ef5b6.BEE3874468EF5B6!666"", 
                 ""from"": {
                    ""name"": ""Jack Jill"", 
                    ""id"": ""0bee3874468ef5b6""
                 }, 
                 ""name"": ""Videos"", 
                 ""description"": null, 
                 ""parent_id"": ""folder.0bee3874468ef5b6"", 
                 ""upload_location"": ""https://apis.live.net/v5.0/folder.0bee3874468ef5b6.BEE3874468EF5B6!666/files/"", 
                 ""is_embeddable"": true, 
                 ""count"": 1, 
                 ""link"": ""https://skydrive.live.com/redir.aspx?cid\u003d0bee3874468ef5b6\u0026page\u003dview\u0026resid\u003dBEE3874468EF5B6!666\u0026parid\u003dBEE3874468EF5B6!103"", 
                 ""type"": ""album"", 
                 ""shared_with"": {
                    ""access"": ""Just me""
                 }, 
                 ""created_time"": ""2012-02-05T05:12:47+0000"", 
                 ""updated_time"": ""2012-02-05T05:31:45+0000""
              }, {
                 ""id"": ""folder.0bee3874468ef5b6.BEE3874468EF5B6!104"", 
                 ""from"": {
                    ""name"": ""Jack Jill"", 
                    ""id"": ""0bee3874468ef5b6""
                 }, 
                 ""name"": ""Documents"", 
                 ""description"": null, 
                 ""parent_id"": ""folder.0bee3874468ef5b6"", 
                 ""upload_location"": ""https://apis.live.net/v5.0/folder.0bee3874468ef5b6.BEE3874468EF5B6!104/files/"", 
                 ""is_embeddable"": true, 
                 ""count"": 2, 
                 ""link"": ""https://skydrive.live.com/redir.aspx?cid\u003d0bee3874468ef5b6\u0026page\u003dview\u0026resid\u003dBEE3874468EF5B6!104\u0026parid\u003dBEE3874468EF5B6!103"", 
                 ""type"": ""folder"", 
                 ""shared_with"": {
                    ""access"": ""Just me""
                 }, 
                 ""created_time"": ""2010-06-16T20:16:52+0000"", 
                 ""updated_time"": ""2012-02-29T05:40:00+0000""
              }, {
                 ""id"": ""file.0bee3874468ef5b6.BEE3874468EF5B6!765"", 
                 ""from"": {
                    ""name"": ""Jack Jill"", 
                    ""id"": ""0bee3874468ef5b6""
                 }, 
                 ""name"": ""Photo 1 #@!+.jpg"", 
                 ""description"": null, 
                 ""parent_id"": ""folder.0bee3874468ef5b6"", 
                 ""size"": 197499, 
                 ""comments_count"": 0, 
                 ""comments_enabled"": false, 
                 ""tags_count"": 0, 
                 ""tags_enabled"": true, 
                 ""is_embeddable"": true, 
                 ""picture"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-fa5e301b-7dda-44a7-8ba6-a43ec401f30b.jpg:Thumbnail/PhotoChooser-fa5e301b-7dda-44a7-8ba6-a43ec401f30b.jpg"", 
                 ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-fa5e301b-7dda-44a7-8ba6-a43ec401f30b.jpg:Default,Largest/PhotoChooser-fa5e301b-7dda-44a7-8ba6-a43ec401f30b.jpg"", 
                 ""upload_location"": ""https://beta.apis.live.net/v5.0/file.0bee3874468ef5b6.BEE3874468EF5B6!765/content/"", 
                 ""images"": [
                    {
                       ""height"": 600, 
                       ""width"": 800, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-fa5e301b-7dda-44a7-8ba6-a43ec401f30b.jpg:Scaled800,WebReady/PhotoChooser-fa5e301b-7dda-44a7-8ba6-a43ec401f30b.jpg"", 
                       ""type"": ""normal""
                    }, {
                       ""height"": 132, 
                       ""width"": 176, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-fa5e301b-7dda-44a7-8ba6-a43ec401f30b.jpg:MobileReady/PhotoChooser-fa5e301b-7dda-44a7-8ba6-a43ec401f30b.jpg"", 
                       ""type"": ""album""
                    }, {
                       ""height"": 72, 
                       ""width"": 96, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-fa5e301b-7dda-44a7-8ba6-a43ec401f30b.jpg:Thumbnail/PhotoChooser-fa5e301b-7dda-44a7-8ba6-a43ec401f30b.jpg"", 
                       ""type"": ""thumbnail""
                    }, {
                       ""height"": 600, 
                       ""width"": 800, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-fa5e301b-7dda-44a7-8ba6-a43ec401f30b.jpg:Default,Largest/PhotoChooser-fa5e301b-7dda-44a7-8ba6-a43ec401f30b.jpg"", 
                       ""type"": ""full""
                    }
                 ], 
                 ""link"": ""https://skydrive.live.com/redir.aspx?cid\u003d0bee3874468ef5b6\u0026page\u003dview\u0026resid\u003dBEE3874468EF5B6!765\u0026parid\u003dBEE3874468EF5B6!103"", 
                 ""when_taken"": ""2010-07-17T00:27:58+0000"", 
                 ""height"": 600, 
                 ""width"": 800, 
                 ""type"": ""photo"", 
                 ""location"": null, 
                 ""shared_with"": {
                    ""access"": ""Just me""
                 }, 
                 ""created_time"": ""2012-03-01T17:37:41+0000"", 
                 ""updated_time"": ""2012-03-01T17:37:41+0000""
              }, {
                 ""id"": ""file.0bee3874468ef5b6.BEE3874468EF5B6!762"", 
                 ""from"": {
                    ""name"": ""Jack Jill"", 
                    ""id"": ""0bee3874468ef5b6""
                 }, 
                 ""name"": ""PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg"", 
                 ""description"": null, 
                 ""parent_id"": ""folder.0bee3874468ef5b6"", 
                 ""size"": 85524, 
                 ""comments_count"": 0, 
                 ""comments_enabled"": false, 
                 ""tags_count"": 0, 
                 ""tags_enabled"": true, 
                 ""is_embeddable"": true, 
                 ""picture"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg:Thumbnail/PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg"", 
                 ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg:Default,Largest/PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg"", 
                 ""upload_location"": ""https://beta.apis.live.net/v5.0/file.0bee3874468ef5b6.BEE3874468EF5B6!762/content/"", 
                 ""images"": [
                    {
                       ""height"": 600, 
                       ""width"": 800, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg:Scaled800,WebReady/PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg"", 
                       ""type"": ""normal""
                    }, {
                       ""height"": 132, 
                       ""width"": 176, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg:MobileReady/PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg"", 
                       ""type"": ""album""
                    }, {
                       ""height"": 72, 
                       ""width"": 96, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg:Thumbnail/PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg"", 
                       ""type"": ""thumbnail""
                    }, {
                       ""height"": 600, 
                       ""width"": 800, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg:Default,Largest/PhotoChooser-6e51aae5-e2c1-4b78-a2d3-9533e5e13ff9.jpg"", 
                       ""type"": ""full""
                    }
                 ], 
                 ""link"": ""https://skydrive.live.com/redir.aspx?cid\u003d0bee3874468ef5b6\u0026page\u003dview\u0026resid\u003dBEE3874468EF5B6!762\u0026parid\u003dBEE3874468EF5B6!103"", 
                 ""when_taken"": ""2010-07-18T02:37:06+0000"", 
                 ""height"": 600, 
                 ""width"": 800, 
                 ""type"": ""photo"", 
                 ""location"": null, 
                 ""shared_with"": {
                    ""access"": ""Just me""
                 }, 
                 ""created_time"": ""2012-02-29T06:33:21+0000"", 
                 ""updated_time"": ""2012-02-29T06:33:21+0000""
              }, {
                 ""id"": ""file.0bee3874468ef5b6.BEE3874468EF5B6!759"", 
                 ""from"": {
                    ""name"": ""Jack Jill"", 
                    ""id"": ""0bee3874468ef5b6""
                 }, 
                 ""name"": ""PhotoChooser中文名.jpg"", 
                 ""description"": null, 
                 ""parent_id"": ""folder.0bee3874468ef5b6"", 
                 ""size"": 183028, 
                 ""comments_count"": 0, 
                 ""comments_enabled"": false, 
                 ""tags_count"": 0, 
                 ""tags_enabled"": true, 
                 ""is_embeddable"": true, 
                 ""picture"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg:Thumbnail/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg"", 
                 ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg:Default,Largest/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg"", 
                 ""upload_location"": ""https://beta.apis.live.net/v5.0/file.0bee3874468ef5b6.BEE3874468EF5B6!759/content/"", 
                 ""images"": [
                    {
                       ""height"": 600, 
                       ""width"": 800, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg:Scaled800,WebReady/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg"", 
                       ""type"": ""normal""
                    }, {
                       ""height"": 132, 
                       ""width"": 176, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg:MobileReady/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg"", 
                       ""type"": ""album""
                    }, {
                       ""height"": 72, 
                       ""width"": 96, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg:Thumbnail/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg"", 
                       ""type"": ""thumbnail""
                    }, {
                       ""height"": 600, 
                       ""width"": 800, 
                       ""source"": ""http://storage.live.com/s1pZ5_vqBc_tkc-IjxJNykpvvdDvZsoRlOTvV7IEZGBET2kt5wug6-m8T_88X_9eashlzU1JLt_bSfh6M-XKsmmI3h5n1AEwJMr8dyxcmTuwxIeQEb7Lv6hyQ/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg:Default,Largest/PhotoChooser-19db671d-8f2a-4914-851c-3536fbca0c84.jpg"", 
                       ""type"": ""full""
                    }
                 ], 
                 ""link"": ""https://skydrive.live.com/redir.aspx?cid\u003d0bee3874468ef5b6\u0026page\u003dview\u0026resid\u003dBEE3874468EF5B6!759\u0026parid\u003dBEE3874468EF5B6!103"", 
                 ""when_taken"": ""2010-07-19T03:39:24+0000"", 
                 ""height"": 600, 
                 ""width"": 800, 
                 ""type"": ""photo"", 
                 ""location"": null, 
                 ""shared_with"": {
                    ""access"": ""Just me""
                 }, 
                 ""created_time"": ""2012-02-29T06:28:23+0000"", 
                 ""updated_time"": ""2012-02-29T06:28:23+0000""
              }
           ]
        }";
        
        public static readonly string ErrorInvalidMethod =
            @"{
               ""error"": {
                  ""code"": ""request_method_invalid"", 
                  ""message"": ""The provided HTTP method 'POST' is not allowed.""
               }
              }";

        public static readonly string ErrorUnAuthorized =
            @"{
                ""error"": {
                    ""code"": ""request_token_unauthorized"", 
                    ""message"": ""The provided access token does not have access to this resource. An access token with one of the following scopes is required: 'wl.contacts_update', 'wl.basic'.""
                }
              }";

        public static readonly string RefreshTokenResponse =
            @"{{
                ""access_token"": ""{0}"",
                ""expires_in"": 3600,
                ""refresh_token"": ""{1}"",
                ""scope"": ""{2}"",
                ""token_type"": ""bearer""
              }}";
    }
}
