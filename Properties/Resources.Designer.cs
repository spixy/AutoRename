﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AutoRename.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AutoRename.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to config.ini.
        /// </summary>
        internal static string ConfigFile {
            get {
                return ResourceManager.GetString("ConfigFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://github.com/spixy/AutoRename.
        /// </summary>
        internal static string HomePage {
            get {
                return ResourceManager.GetString("HomePage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/spixy/AutoRename/master/lastversion.
        /// </summary>
        internal static string UpdateFile {
            get {
                return ResourceManager.GetString("UpdateFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Update to\r\nnew version.
        /// </summary>
        internal static string WebsiteButtonUpdate {
            get {
                return ResourceManager.GetString("WebsiteButtonUpdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Visit\r\nwebsite.
        /// </summary>
        internal static string WebsiteButtonVisit {
            get {
                return ResourceManager.GetString("WebsiteButtonVisit", resourceCulture);
            }
        }
    }
}
