1������ʹ��Memcached������Ҫ�������£�
	a����Ҫ��������DLL��
		\lib\Enyim.Caching.dll
	��MemcachedProviders.dll���Բ������ã�����������Զ���binĿ¼���ɴ�dll�ļ���
	b��web.config�ļ���configSections�ڵ�������ã�
		<sectionGroup name="enyim.com">
		  <section name="memcached" type="Enyim.Caching.Configuration.MemcachedClientSection, Enyim.Caching" />
		</sectionGroup>
	c��web.config�ļ���configuration�ڵ�������ã�
		<enyim.com>
			<memcached>
			  <servers>
				<add address="10.0.37.19" port="11211" />
				<add address="10.0.37.20" port="11211" />
			  </servers>
			  <socketPool minPoolSize="10" maxPoolSize="100" connectionTimeout="00:00:10" deadTimeout="00:02:00" />
			</memcached>
		  </enyim.com>

2��XFramework.Data������˵����
	a��XFramework���ݲ��������࣬��Ҫ�Լ���дSQL�ű���������ɾ�Ĳ飬��֧��SQLServer��Oracle��
	b��IDataΪʵ��ELinq�����ݿ������Ļ�����ɾ�Ĳ�Ľӿڡ�

3��XFramework.Log������˵����
	�ٳ���ʹ��Log4Net��¼��־��Ҫ�������£�
		a����Ҫ����DLL���£�
			\lib\log4net.dll
		b����web.config�ļ���configSections�ڵ�������ã�		
			<section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
			<section name="XFrameworkLog" type="XFramework.Log.LogHandler,XFramework"/>
		c����web.config�ļ���configuration�ڵ�������ã�
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
	���������ʹ��MongoDB��¼��־��Ҫ�������£�
		a����Ҫ����DLL���£�
			\lib\MongoDB.Bson.dll
			\lib\MongoDB.Driver.dll
		a����web.config�ļ���configSections�ڵ�������ã�		
			<section name="XFrameworkLog" type="XFramework.Log.LogHandler,XFramework"/>
		b����web.config�ļ���configuration�ڵ�������ã�
			<XFrameworkLog type="XFramework.Log.MongoDbLog,XFramework" appname="XFramework.Web"/>
		c����web.config�ļ���appSettings�ڵ�������ã�
			<add key="XFrameworkMongoDBLog.DataBaseName" value="MolaDb"></add>
			<add key="XFrameworkMongoDBLog.TableName.Error" value="MolaLog_Error"></add>
			<add key="XFrameworkMongoDBLog.TableName.Debug" value="MolaLog_Debug"></add>
			<add key="XFrameworkMongoDBLog.TableName.Warn" value="MolaLog_Warn"></add>
			<add key="XFrameworkMongoDBLog.TableName.Fatal" value="MolaLog_Fatal"></add>
		d����web.config�ļ���connectionStrings�ڵ�������ã�
			<add name="XFrameworkMongoDBLog" connectionString="mongodb://10.0.33.14:27017" providerName="MongoDB.Driver.MongoClient" />