h-opc tests
================

# Configuration

To configure the test project, ensure that there is an App.config file in the project directory with the following (or similar) contents:

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="UATestEndpoint" value="opc.tcp://localhost:61210/UA/SampleServer"/>
  </appSettings>
</configuration>
```
