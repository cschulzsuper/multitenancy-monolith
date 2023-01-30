# Roles

## Identity Roles

* `admin`

  The administrator of the application has access to the identity endpoints. 

  Condition: `Identity == "admin"`

* `default`

  Normal identities can sign in as a member. 

  Condition: `!string.IsNullOrEmpty(Identity)`

* `secure`

  Secure identities can modify personal identity settings and the identity secret.

  Condition: `Identity != "demo"`

## Member Roles

* `chief`

  The chief of a group has full access to the member endpoints. 

  Condition: `Member.StartsWith("chief-")`

* `member`

  Normal members have limited access to the member endpoints. 

  Condition: `!string.IsNullOrEmpty(Member)`

* `observer`

  Observers can only access HTTP GET endpoints. 

  Condition:  `Identity == "demo" && !string.IsNullOrEmpty(Member)`