
### ��ȡ�����Լ�һЩ��������

##### ��ȡ�����ļ�
```
Install-Package jfYu.Core.Common
```
 

##### �ӽ���
 using jfYu.Core.Common.Utilities


```
//Aes�ӽ���
"test".AesEncrypt().AesEncrypt();
//Des�ӽ���
"test".DesEncrypt().DesEncrypt();
//Ras�ӽ���
Rsa.Encrypt("test", "d://pems/RSA.Pub")
Rsa.Decrypt("testEncryptString", "d://pems/RSA.Pub");

//SHA
//MD5
"test".SHAmd5Encrypt();
//SHA1
"test".SHA1Encrypt();
//SHA256
"test".SHA256Encrypt();
//SHA384
"test".SHA384Encrypt();
//SHA512
"test".SHA384Encrypt();

```

##### DataTableתList

```
var dt=new DataTable();
var list=dt.ToModels<T>();

var dr=dt.Rows[0];
T model=dt.ToModel<T>();

```

##### ���л�

```
var res=Serializer.Deserialize<T>(json)
var res=Serializer.Serialize(object)

```

##### unixʱ���

```
UnixTime.GetUnixTime();//��ȡ��ǰʱ���

UnixTime.GetUnixTime(DateTime.Now);//��ȡָ��ʱ���

UnixTime.GetDateTime(unixtime);//ʱ���תDateTime

```

##### д���ļ���־

```
//���ֶ�IOCע�����ֱ��new
//log�ļ������Ŀ¼Logsÿ��һ��
 WriteFileLog log=new WriteFileLog();
 log.WriteLog("xxxx");//д��log
 log.WriteLog("xxxx","logxxx");//д��log����Ӧ���Ƶ�log�ļ���

```


##### AutoMapper


```
//ע��
var containerBuilder = new ContainerBuilder();
containerBuilder.AddAutoMapper(cfg =>
{
    cfg.CreateMap<User, UserProfileViewModel>().ForMember(q => q.AllName, opt => opt.MapFrom(q => q.NickName + q.UserName));
});

var c = containerBuilder.Build();
var am = c.Resolve<IMapper>();

//ʹ��
  var u = new User() { UserName = "x", NickName = "y" };
  var vu = am.Map<UserProfileViewModel>(u);
```