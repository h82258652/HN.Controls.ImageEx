# HN.Controls.ImageEx
a cached Image control for WPF and UWP.<br/>
WPF version for .net framework 4.7.1<br/>
UWP version for 16299<br/>
but both you can downgrade, the source code is here.😀

usage please see the Demo project.

and WPF must add this code in App.config file.
```XML
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="System.Net.Http" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-4.2.0.0" newVersion="4.1.1.2" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>
```