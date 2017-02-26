# Simple Token Service for ASP.NET Core

<h3>Synopsis</h3>

This is an example of implementation of JSON Web Token (JWT) authentification for ASP Core Web API. It is based on [SimpleTokenProvider](https://github.com/nbarbettini/SimpleTokenProvider). Some of ideas were taken from [openiddict-samples](https://github.com/openiddict/openiddict-samples). It has the similar implementation as SimpleTokenProvider but as service with a controller. In that repo you may get ideas how to implement it in your own project.

<h3>Motivation</h3>

I wanted to create self made implementaion of JWT authentification as in SimpleTokenProvider but not using midleware. 

<h3>Installation</h3>

* Clone repo
* Add user secrets:
```
{
  "TokenKey": "MySuperSecret_!123",
  "TokenIssuer": "issuer",
  "TokenAudience": "audience"
}
```
* Generate database using migrations
* Create user using ASP Identity

* Send POST request to that URL: http://localhost:49940/api/auth

HEADER:
``` 
Content-Type: application/x-www-form-urlencoded
```
BODY:
```
username: YourUserName
password: YourPassword
 ```
