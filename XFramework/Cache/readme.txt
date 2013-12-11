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

2）缓存的对象需要序列化才能保存到Memcached缓存服务器中。
	当我们在应用中使用分布式缓存memcached，此时数据被缓存在应用程序之外的进程中。每次，当我们要把一些数据缓存起来的时候，缓存的API就会把数据首先序列化为字节的形式，然后把这些字节发送给缓存服务器去保存。
	同理，当我们在应用中要再次使用缓存的数据的时候，缓存服务器就会将缓存的字节发送给应用程序，而缓存的客户端类库接受到这些字节之后就要进行反序列化的操作了，将之转换为我们需要的数据对象。
    这个序列化与反序列化的机制都是发生在应用程序服务器上的，而缓存服务器只是负责保存而已。
    .NET中的默认使用的序列化机制不是最优的，因为它要使用反射机制，而反射机制是是非常耗CPU的，特别是当我们缓存了比较复杂的数据对象的时候。
　　基于这个问题，我们要自己选择一个比较好的序列化方法来尽可能的减少对CPU的使用。常用的方法就是让对象自己来实现ISerializable接口。

	实例参考：
	[Serializable]
    public class Product
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Desc { get; set; }
    }

    public class Product : ISerializable
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Desc { get; set; }

        public Product(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Price = info.GetDecimal("Price");
            Desc = info.GetString("Desc");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Price", Price);
            info.AddValue("Desc", Desc);
        }
    }

	自己实现的方式与.NET默认的序列化机制的最大区别在于：没有使用反射。自己实现的这种方式速度可以是默认机制的上百倍。