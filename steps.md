# Azure Active Directory
![image](https://user-images.githubusercontent.com/52662114/112529395-82e38080-8da5-11eb-8ad3-717b385818db.png)


# Create a new tenant 
1. Sign in to your organization's Azure portal.
2. From the Azure portal menu, select Azure Active Directory.
3. Select Azure Active Directory - Overview page -> Create a tenant
4. On the Basics tab, select the type of tenant you want to create, either Azure Active Directory or Azure Active Directory (B2C). Select Azure Active Directory.
5. Select Next: Configuration to move on to the Configuration tab.
6. On the Configuration tab, enter the following information:
	- Type **Tenant Name** into the Organization name box
	- Type **Domain Name** into the Initial domain name box
	- Select North Macedonia option in the Country or region box 
7. Select Next: Review + Create. Review the information you entered and if the information is correct, select create.

Your new tenant is created with the domain _yourdomainname_.onmicrosoft.com.

## Switch to our created tenant (directory)

# Create Web App 
In order to deploy our application to Azure, we need to create Web App resource.

1. On the Basics tab enter the required fields:
		- Select your subscription in Subscription field
		- Select your resource group or create a new one
		- Add name for your application in Name field
		- In the Publish field select Code
		- for runtime stack select .NET Core 3.1 since we will deploy .net core app
2. Click Next to go on the Monitor tab
		- For Enable Application Insights select No, we don't want App Insights resource to be created along with the web app creation

3. Click on Review+Create, and then Create.
4. After the web app resource is created, click on Go the resource
	- In Overview tab, click on the Web App URL to check if the app is started

## Publish our application to Azure

1. After building your application in Visual Studion, click right click on the project name and the Publish
2. On the Publish page, click on NEW link to create new Publish profile
3. On the Pick a Publish Target screen, select App Service, and then Select Existing option and click on Create profile button. (Since we created a App Service from Azure we are chosing the Existing option, if not we can select Create New option to create new App Service along with the publishing)
![image](https://user-images.githubusercontent.com/52662114/112001520-54ed0a80-8b1f-11eb-9f5f-d40758252509.png)
4. On the next screen, choose your subscription and resource group, and then select your created web app resource.
5. Click on Publish button.
6. When publishing is done, the web app site will open in a browser with the URL provided on Azure


# Enable Authentication for the app from the Azure portal


![image](https://user-images.githubusercontent.com/52662114/112531111-7bbd7200-8da7-11eb-951e-6ea822368518.png)


1. Click on Authentication/Authorization tab on the App Service
2. Select On for App Service Authentication.
3. For Authentication Providers, click on the Azure Active Directory option to configure authentication.
4. On the Active Directory Authentication page, choose Express for Management mode for automatically creating app registration 
5. In Management mode leave Create New AD App option selected
6. Enter name for the App Registration in Create App field
7. Click OK.
8. Then for Action to take when request is not authenticated option choose _Log in with Azure Active Directory_
9. Click on Save
10. On the App Service-> Overview tab, click again on the app URL after enabling Authentication
11. Log in with the email loged in in the Azure portal
12. Sign the consent screen for permissions
13. The Home Page then is loaded in the web app

_(Informative only)_ 
Another option for creating App Registration is:
1. On the Home Screen, go to Azure Active Directory resource
2. On the App registrations tab, click on New Registration
	- Enter Name for the app registration
	- Choose Supported account types for single or multi tenant
3. Click on Register


### Change the code to support AAD
1. Add the following code in ConfigureServices method before _services.AddControllersWithViews();_ line in the Startup class for securing the app with OpenID Connect provider
```
	    services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://login.microsoftonline.com/your_application_tenant_id";          
                    options.ClientId = "your_application_ID";
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RoleClaimType = "roles",
                    };
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                });
```

2. Add **[Authorize]** attribute to the HomeController

**Get the App Registration data**
1. On the Home Screen, go to Azure Active Directory resource
2. Click on App Registration tab
3. Choose our created app registration from the previous step
4. Get the _Application (client) ID_ and add it to the ClientId option on the Autchentication options in the code on Configure Services method
5. Get Directory (tenant) ID and add it to Authority option on the Autchentication options in the code on Configure Services method

### Publish the app again on Azure
1. Right click on the project name -> Publish
2. Since we already configured the publish profile, click directly on the Publish button
3. If we now try to login into the page we will get an error saying that Redirect URL does not match to our application ID

### Change Authentication option on the App Service since we have now added Authentication from the code
1. On the Azure Active Directory -> App Registrations -> Our app registration, click on the Authentication tab
2. In the Redirect URIs for WEB, change the URI to be: https://your_application_URL/signin-oidc, since we have enabled OpenID Connect from the code (the default URL will work for .net applications but not .net core app)
3. Click on Save
4. Go to your Web App Service, click on Authentication tab and disable the App Service Authentication
5. Click on Overview tab and restart your application
6. Click on the web app URL and try to login


# RBAC (Role-based Access control)
![image](https://user-images.githubusercontent.com/52662114/112533679-7f9ec380-8daa-11eb-8c96-87e008fa75fa.png)


## Create Users
1. Search Users on the Search form
2. On the Users page click on the New user link
3. On the New User page enter the following fields:
	- Add User name for the new user we are creating. Leave the domain name as default, since that is our tenant domain name
	- Add a Name for the user
	- Enter first anf last name
	- On the Password options, choose Let me create the password and enter password for the user
	- Dont choose anything on groups and roles yet since we haven't created roles
	- Click on Create button
		
## Change the manifest to add multiple roles
1. On the Azure Active Directory -> App Registrations -> Click on our app registration, click on Manifest
2. Add the following code to the appRoles property of the manifest:
```
[
		{
			"allowedMemberTypes": [
				"User"
			],
			"description": "Administrator is able to do everything in the app",
			"displayName": "Admin",
			"id": "d1c2ade8-0000-0000-0000-6d06b947c66f",
			"isEnabled": true,
			"lang": null,
			"origin": "Application",
			"value": "Administrator"
		},
		{
			"allowedMemberTypes": [
				"User"
			],
			"description": "Reader is another role",
			"displayName": "Reader",
			"id": "d1c2ade8-0000-0000-0000-6d06b947c67f",
			"isEnabled": true,
			"lang": null,
			"origin": "Application",
			"value": "Reader"
		}
	]
```

3. Note: change the GUID id of the both roles added
4. Click on Save button

## Assign our users to the roles
1. On the Azure Active Directory -> Click on Enterprise application tab
2. Choose our app registration
3. On the Users and groups tab, click on Add user/group to assign our application users to the newly created roles
4. On the Add Assignment page: 
	- select Users link and first add the owner logged in the Azure portal (current user)
	- select a role and add Admin role to that user
6. Repeat the last step for the latest created User from the previous step and add him the Reader role
7. Log in into the web app with the newly created user and see his role on the Claims screen, and check that he can access all of the pages

## Change the code to support RBAC
In order to show the RBAC and the roles configured for our application, we will make the following changes to the code:
1. In the Startup file, configure Authorization with adding the following code after AddAutchentication coode in ConfigureServices method:
```
	services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy",
                    policy =>
                    {
                        policy.AddRequirements(new AdminRequirement());
                        policy.RequireAuthenticatedUser();
                        policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
                    });

                options.AddPolicy("ReaderPolicy",
                    policy =>
                    {
                        policy.AddRequirements(new ReaderRequirement());
                        policy.RequireAuthenticatedUser();
                        policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
                    });
            });
```

Finally the method should look like:
```
public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://login.microsoftonline.com/your_tenant_id";
                    options.ClientId = "your_client_id"; 
                    options.ResponseType = OpenIdConnectResponseType.IdToken;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RoleClaimType = "roles",
                    };
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy",
                    policy =>
                    {
                        policy.AddRequirements(new AdminRequirement());
                        policy.RequireAuthenticatedUser();
                        policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
                    });

                options.AddPolicy("ReaderPolicy",
                    policy =>
                    {
                        policy.AddRequirements(new ReaderRequirement());
                        policy.RequireAuthenticatedUser();
                        policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
                    });
            });


            services.AddControllersWithViews();
        }
```
3. Add the following attribute: **[Authorize(Policy = "ReaderPolicy")]** to the Contact method of the HomeController.
	- with adding this attribute, we are saying that only the users that have Reader role can access this page, if not Access Denied page will be thrown

## Publish the new changes
Build the project and publish it again

After publishing the changes, log into the application with both users, the Admin and the Reader and check their access to the pages.

# Setup Multi Factor Authentication for user
1. Go to Users page
2. Click on the Multi-factor Authentication link
3. Another page will be opened and you need to log in with the owner users loged in on Azure portal
4. On the new page, you can see that multi-factor authentication has been disabled for all of the users
 ![image](https://user-images.githubusercontent.com/52662114/112025001-077b9800-8b35-11eb-9c87-27ba80ab4def.png)
5. Select the user you want to enable multi-factor authentication for
6. From quick steps, choose Enable option and then again Enable
7. Go back to your website and try to login with the user you enable MFA for
8. Setup MFA for that user with Microsoft Authenticator on your phone
9. After successfull setup of MFA, try log out and login again to see that MFA is enabled for that user


# BONUS
# RBAC - Service principal
![image](https://user-images.githubusercontent.com/52662114/112330015-f7d88c80-8cb7-11eb-86fb-6a8c66d24bfc.png)

## Create Key vault 
1. Search Key Vault in Azure search bar, and click on Add Key Vault
2. Coose your subscription and resource group
3. Enter Instance details:
	- Key vault name
	- Region
	- Pricing tier (Leave it as Standard)
4. Click on Review+create
5. Click on Create
6. After creating of the resourcec, click on the Go to Resource button
7. On the KeyVault page, click on Secrets and then Generate/Import button
8. Add a new secret and define Secret_Name

## Create Service principal for our application
1. Open Azure cloud bash/powershell
2. Create new service principal using az commands
	az ad sp create-for-rbac -n name --skip-assignment
3. Copy the output returned of the commant
	- appId: client id of the service principal
	- password: client secret 
	- tenant: our tenant id
5. We need to assign permission policy for access for the service principal, so the service principal can access the secrets into the key vault
	az keyvault set-policy --name keyvaultname --spn "appId_of_sp" --secret-permissions backup delete get list set
	
## Add code changes to read secret from Key Vault
1. Add the following methods in HomeController
```
 string GetVaultValue()
        {
            KeyVaultClient client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));
            var vaultAddress = "https://keyvaultname.vault.azure.net/";

            var secretName = "SECRET_NAME_CREATED_IN_KEYVAULT";

            var secret = client.GetSecretAsync(vaultAddress, secretName).GetAwaiter().GetResult();
            return secret.Value;
        }

        async Task<string> GetToken(string authority, string resource, string scope)
        {
            var clientId = "APP_ID_OF_THE_SERVICE_PRINCIPAL";
            var clientSecret = "CLIENT_SECRET_OF_SP";
	    
            Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential credential = new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(clientId, clientSecret);
            var context = new AuthenticationContext(authority, Microsoft.IdentityModel.Clients.ActiveDirectory.TokenCache.DefaultShared);

            var result = await context.AcquireTokenAsync(resource, credential);
            return result.AccessToken;
        }
```
2. Change the secret_name, appId and client secret in the code above with the values created before.
3. Add the following two changes in the About method in Home Controller, so we can display the secret value of a secret from KeyVault on UI
```
var secret = GetVaultValue();
ViewBag.Message = $"Hey {userfirstname}! Welcome {User.FindFirstValue(ClaimTypes.Role)} + {secret}";
```
4. Publish the web app again on Azure
5. Login into the app and click on About to see the secret is get from key vault


# SSL TLS
1. On the App Service, click on TLS/SSL settings tab
2. If you are on free tier you need to upgrade your App Service Plan to Basic tier 
		- click on the info message to upgrade your App Service Plan/ Click on Scale up(App Service plan) tab
		- Click on B1 tier (Basic)
		- Click on Apply
3. On TLS/SSL settings tab, choose ON option for HTTPS Only.
4. Choose TLS 1.2 option

