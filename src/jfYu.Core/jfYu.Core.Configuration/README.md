
### <a href="#Configuration">Configuration���ù���</a>
```
1���Զ���ȡĿ¼�������ļ������ݻ���������ȡ�����ļ������ȡ���������ļ���
2��ѡ��ģʽ�����߼�������Options��β����ȥ��Options�ַ���������Ϊ������

```
Nuget��װ

```
Install-Package jfYu.Core.Configuration
```

ʹ��

```
//��������
new ConfigurationBuilder().AddConfigurationFiles(); //��������
new ConfigurationBuilder().AddConfigurationFiles(new MyHostEnvironment());//��������������
new ConfigurationBuilder().AddConfigurationFile("CacheRedis.json", true, true);//���ص��������ļ�

//��ȡ����
//����ģʽ
AppConfig.Configuration["Cache:Type"]
AppConfig.Configuration.GetSection("Redis").GetSection("Timeout").Value
//ѡ��ģʽ Class�̳�IConfigurableOptions

public class RedisOptions : IConfigurableOptions
{

	public List<RedisEndPoint> EndPoints { get; set; }
	/// <summary>
	/// ����
	/// </summary>

	public string Password { get; set; }

	/// <summary>
	/// ���ݿ�index
	/// </summary>

	public int DbIndex { get; set; } = 0;

	/// <summary>
	/// ��ʱʱ�䣨���룩
	/// </summary>

	public int Timeout { get; set; } = 5000;

}

public class RedisEndPoint
{
	/// <summary>
	/// ����
	/// </summary>
	public string Host { get; set; }

	/// <summary>
	/// �˿�
	/// </summary>
	public int Port { get; set; }
}
//IOCע��
 var services = new ServiceCollection();
 services.AddConfigurableOptions<RedisOptions>();
 //IOC��ȡ
 ContainerBuilder containerBuild = new ContainerBuilder();
 containerBuild.Populate(services);
 containerBuild.RegisterType<OptionServer>();
 var container = containerBuild.Build();
 var options = container.Resolve<OptionServer>();

```