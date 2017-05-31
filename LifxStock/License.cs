using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace LifxStock
{
    [Activity(Label = "License")]
    public class License : AppCompatActivity
    {
        #region Properties

        private TextView licenseTextView;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.License);

            FindViews();

            SetupToolbar();

            SetLicenseText();
        }

        private void SetLicenseText()
        {
            var licenseText = GetLicenseXamSettings();
            licenseText += "<br/>";
            licenseText += GetLicenseCsvHelper();
            licenseText += "<br/>";
            licenseText += GetLicenseLifxHttp();
            licenseText += "<br/>";
            licenseText += GetLicenseMonoDroidExamples();
            licenseText += "<br/>";
            licenseText += GetLicenseHTextView();
            licenseText += "<br/>";
            licenseText += GetLicenseAkavache();

            licenseTextView.SetText(Html.FromHtml(licenseText), TextView.BufferType.Spannable);

            licenseTextView.MovementMethod = Android.Text.Method.LinkMovementMethod.Instance;
        }

        private string GetLicenseAkavache()
        {
            return @"<h3>Akavache</h3>
                    <p>Copyright (c) 2012 GitHub (flagbug) <a href=""https://github.com/akavache/Akavache"">https://github.com/akavache/Akavache</a></p>
                    <p>Permission is hereby granted, free of charge, to any person obtaining a<br />copy of this software and associated documentation files (the ""Software""),<br />to deal in the Software without restriction, including without limitation<br />the rights to use, copy, modify, merge, publish, distribute, sublicense,<br />and/or sell copies of the Software, and to permit persons to whom the<br />Software is furnished to do so, subject to the following conditions:</p>
                    <p>The above copyright notice and this permission notice shall be included in<br/> all copies or substantial portions of the Software.</p>
                    <p>THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR<br/> IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,<br/> FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE <br/> AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER<br/> LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING<br/> FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER <br/> DEALINGS IN THE SOFTWARE.</p> ";
        }

        #region SetupFunctions

        private void SetupToolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);

            SupportActionBar.Title = "Licenses";

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        #endregion

        private void FindViews()
        {
            licenseTextView = FindViewById<TextView>(Resource.Id.licenseTextView);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
                Finish();

            return base.OnOptionsItemSelected(item);
        }

        public string GetLicenseHTextView()
        {
            return @"<h3>HTextView</h3>
                    <p>Copyright (C) 2015 [Hanks] <a href=""https://github.com/hanks-zyh"">https://github.com/hanks-zyh</a></p>
                    <p>Licensed under the Apache License, Version 2.0 (the ""License"");<br /> you may not use this file except in compliance with the License.<br /> You may obtain a copy of the License at</p>
                    <p><a href=""http://www.apache.org/licenses/LICENSE-2.0"">http://www.apache.org/licenses/LICENSE-2.0</a></p>
                    <p>Unless required by applicable law or agreed to in writing, software<br /> distributed under the License is distributed on an ""AS IS"" BASIS,<br /> WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.<br /> See the License for the specific language governing permissions and<br /> limitations under the License.</p>";
        }

        public string GetLicenseMonoDroidExamples()
        {
            return @"<h3>MonoDroid Samples - Searchable Directory</h3>
                    <p><a href=""https://github.com/xamarin/monodroid-samples"">https://github.com/xamarin/monodroid-samples</a></p>
                    <p>Copyright 2011 Xamarin Inc</p>
                    <p>Licensed under the Apache License, Version 2.0 (the ""License"");<br /> you may not use this file except in compliance with the License.<br /> You may obtain a copy of the License at</p>
                    <p><a href=""http://www.apache.org/licenses/LICENSE-2.0"">http://www.apache.org/licenses/LICENSE-2.0</a></p>
                    <p>Unless required by applicable law or agreed to in writing, software<br /> distributed under the License is distributed on an ""AS IS"" BASIS,<br /> WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.<br /> See the License for the specific language governing permissions and<br /> limitations under the License.</p>";
        }

        public string GetLicenseLifxHttp()
        {
            return @"<h3>LifxHttpNet</h3>
                    <p><a href=""https://github.com/mensly/LifxHttpNet"">https://github.com/mensly/LifxHttpNet</a></p><br/>
                    <p>Copyright (c) 2015 Michael Ensly</p>
                    <p>Permission is hereby granted, free of charge, to any person obtaining a copy<br />of this software and associated documentation files (the ""Software""), to deal<br />in the Software without restriction, including without limitation the rights<br />to use, copy, modify, merge, publish, distribute, sublicense, and/or sell<br />copies of the Software, and to permit persons to whom the Software is<br />furnished to do so, subject to the following conditions:</p>
                    <p>The above copyright notice and this permission notice shall be included in<br />all copies or substantial portions of the Software.</p>
                    <p>THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR<br />IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,<br />FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE<br />AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER<br />LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,<br />OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN<br />THE SOFTWARE</p>";
        }

        public string GetLicenseCsvHelper()
        {
            var csvHelper = @"<h3>CsvHelper</h3>
                     <p><a href=""https://github.com/joshclose/csvhelper"">https://github.com/joshclose/csvhelper</a></p><br/>
                     <p>Microsoft Public License (Ms-PL)</p>
                    <p>This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.</p>
                    <p>1. Definitions</p>
                    <p>The terms ""reproduce,"" ""reproduction,"" ""derivative works,"" and ""distribution"" have the same meaning here as under U.S. copyright law.</p>
                    <p>A ""contribution"" is the original software, or any additions or changes to the software.</p>
                    <p>A ""contributor"" is any person that distributes its contribution under this license.</p>
                    <p>""Licensed patents"" are a contributor's patent claims that read directly on its contribution.</p>
                    <p>2. Grant of Rights</p>
                    <p>(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.</p>
                    <p>(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.</p>
                    <p>3. Conditions and Limitations</p>
                    <p>(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.</p>
                    <p>(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.</p>
                    <p>(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.</p>
                    <p>(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.</p>
                    <p>(E) The software is licensed ""as-is."" You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.</p>";

            var csvHelperApache = @"<br/>
                    <p>Copyright 2016 Josh Close</p>
                    <p>Licensed under the Apache License, Version 2.0 (the ""License"");<br /> you may not use this file except in compliance with the License.<br /> You may obtain a copy of the License at</p>
                    <p><a href=""http://www.apache.org/licenses/LICENSE-2.0"">http://www.apache.org/licenses/LICENSE-2.0</a></p>
                    <p>Unless required by applicable law or agreed to in writing, software<br /> distributed under the License is distributed on an ""AS IS"" BASIS,<br /> WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.<br /> See the License for the specific language governing permissions and<br /> limitations under the License.</p>";

            return csvHelper + csvHelperApache;
        }

        public string GetLicenseXamSettings()
        {
            return @"<h3><strong>Xam.Plugins.Settings</strong></h3>
                    <p><strong><a href=""https://github.com/jamesmontemagno/SettingsPlugin"">https://github.com/jamesmontemagno/SettingsPlugin</a></strong></p>
                    <br/>                    
                    <p>The MIT License (MIT)</p>
                    <p> Copyright(c) 2016 James Montemagno</p>
                    <p> Permission is hereby granted, free of charge, to any person obtaining a copy<br/> 
                    of this software and associated documentation files(the ""Software""), to deal<br/>
                    in the Software without restriction, including without limitation the rights<br/>
                    to use, copy, modify, merge, publish, distribute, sublicense, and / or sell <br/> 
                    copies of the Software, and to permit persons to whom the Software is<br/> 
                    furnished to do so, subject to the following conditions:</p>
                    <p> The above copyright notice and this permission notice shall be included in all <br/> 
                    copies or substantial portions of the Software.</p>
                    <p> THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR<br/>
                    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,<br/>
                    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE <br/>
                    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER<br/>
                    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,<br/> 
                    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE <br/> SOFTWARE.</p>
                    <p> &nbsp;</p> ";
        }
    }
}