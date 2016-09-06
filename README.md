# D-YALV! - (Dynamic) Yet Another Log4Net Viewer

YALV! is a log file viewer for Log4Net with handy features like log merging, filtering, open most recently used files, items sorting and so on. It is easy to use, it requires no configuration, it has intuitive and user-friendly interface and available in several languages. It is a WPF Application based on .NET Framework 4.0 and written in C# language.

D-YALV is a fork of YALV! that supports custom columns & data. During your applications execution, just log your custom data as a <log:data> element, like so:

```xml
<log4j:event logger="LoggingService" timestamp="1471208402881" level="DEBUG" thread="30">
  <log4j:message>Testing logs..</log4j:message>
  <log4j:properties>
    <!-- Default log4net columns -->
    <log4j:data name="log4jmachinename" value="user-machine" />
    <log4j:data name="log4net:Identity" value="SYSTEM" />
    <log4j:data name="log4net:UserName" value="SYSTEM\USER" />
    <log4j:data name="log4japp" value="LoggingService.exe" />
    <log4j:data name="log4net:HostName" value="system" />
    
    <!-- Custom columns, added by you -->
    <log4j:data name="userId" value="410" />
    <log4j:data name="locationGuid" value="7FF2DFF98DA8314C8CF428A46FBE4555" />

  </log4j:properties>
<log4j:locationInfo class="..." method="MoveNext" file="..." line="43" />
</log4j:event>
```

and D-YALV will show them just like a default log4net column. 

## Usage
Use it just like how you would use YALV!.
