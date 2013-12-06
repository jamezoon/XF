1）程序使用Memcached缓存需要配置如下：
	a）需要引用以下DLL：
		\lib\Enyim.Caching.dll
	另：MemcachedProviders.dll可以不用引用，程序编译后会自动在bin目录生成此dll文件。
	b）web.config文件的configSections节点添加配置：
		<sectionGroup name="enyim.com">
		  <section name="memcached" type="Enyim.Caching.Configuration.MemcachedClientSection, Enyim.Caching" />
		</sectionGroup>
	c）web.config文件的configuration节点添加配置：
		<enyim.com>
			<memcached>
			  <servers>
				<add address="10.0.37.19" port="11211" />
				<add address="10.0.37.20" port="11211" />
			  </servers>
			  <socketPool minPoolSize="10" maxPoolSize="100" connectionTimeout="00:00:10" deadTimeout="00:02:00" />
			</memcached>
		  </enyim.com>

2）XFramework.Data操作类说明：
	a）XFramework数据操作帮助类，需要自己编写SQL脚本来进行增删改查，可支持SQLServer、Oracle。
	b）IData为实现ELinq的数据库操作类的基本增删改查的接口。

3）XFramework.Log操作类说明：
	①程序使用Log4Net记录日志需要配置如下：
		a）需要引用DLL如下：
			\lib\log4net.dll
		b）在web.config文件的configSections节点添加配置：		
			<section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
			<section name="XFrameworkLog" type="XFramework.Log.LogHandler,XFramework"/>
		c）在web.config文件的configuration节点添加配置：
			<XFrameworkLog type="XFramework.Log.Log4NetLog,XFramework" appname="XFramework.Web"/>
			<log4net>
			<appender name="RollingLogFileAppender" type="log4net.Appender.RollingFileAppender">
			  <param name="File" value="LogFiles/"/>
			  <param name="AppendToFile" value="true"/>
			  <param name="MaxSizeRollBackups" value="10"/>
			  <param name="StaticLogFileName" value="false"/>
			  <param name="DatePattern" value="yyyy-MM-dd&quot;.txt&quot;"/>
			  <param name="RollingStyle" value="Date"/>
			  <layout type="log4net.Layout.PatternLayout">
				<param name="ConversionPattern" value="%d{yyyy-MM-dd HH:mm:ss}[%thread] %-5level %logger %ndc - %message%newline"/>
			  </layout>
			</appender>
			<appender name="ConsoleAppender" type="log4net.Appender.ConsoleAppender">
			  <layout type="log4net.Layout.PatternLayout">
				<param name="ConversionPattern" value="%d{yyyy-MM-dd HH:mm:ss}[%thread] %-5level %logger %ndc - %message%newline" />
			  </layout>
			</appender>
			<root>
			  <level value="WARN" />
			  <appender-ref ref="RollingLogFileAppender" />
			  <appender-ref ref="ConsoleAppender" />
			</root>
			<logger name="Memcached.ClientLibrary">
			  <level value="WARN" />
			</logger>
		  </log4net>
	②如果程序使用MongoDB记录日志需要配置如下：
		a）需要引用DLL如下：
			\lib\MongoDB.Bson.dll
			\lib\MongoDB.Driver.dll
		a）在web.config文件的configSections节点添加配置：		
			<section name="XFrameworkLog" type="XFramework.Log.LogHandler,XFramework"/>
		b）在web.config文件的configuration节点添加配置：
			<XFrameworkLog type="XFramework.Log.MongoDbLog,XFramework" appname="XFramework.Web"/>
		c）在web.config文件的appSettings节点添加配置：
			<add key="XFrameworkMongoDBLog.DataBaseName" value="MolaDb"></add>
			<add key="XFrameworkMongoDBLog.TableName.Error" value="MolaLog_Error"></add>
			<add key="XFrameworkMongoDBLog.TableName.Debug" value="MolaLog_Debug"></add>
			<add key="XFrameworkMongoDBLog.TableName.Warn" value="MolaLog_Warn"></add>
			<add key="XFrameworkMongoDBLog.TableName.Fatal" value="MolaLog_Fatal"></add>
		d）在web.config文件的connectionStrings节点添加配置：
			<add name="XFrameworkMongoDBLog" connectionString="mongodb://10.0.33.14:27017" providerName="MongoDB.Driver.MongoClient" />