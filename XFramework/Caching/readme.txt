程序使用Memcached缓存需要配置如下：
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