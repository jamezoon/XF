����ʹ��Memcached������Ҫ�������£�
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