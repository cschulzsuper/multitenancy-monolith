# Roles

## Identity Roles

* `admin`

  The administrator of the application has access to the identity endpoints. 

  Condition: `AuthenticationIdentity == "admin"`

* `identity`

  Normal identities can sign in as a member. 

  Condition: `!string.IsNullOrEmpty(AuthenticationIdentity)`

* `demo`

  Demo identities have restrictions and can not modify most data.

  Condition: `AuthenticationIdentity != "demo"`

## Member Roles

* `chief`

  The chief of a group has full access to the member endpoints. 

  Condition: `AccountMember.StartsWith("chief-")`

* `chief-observer`

  Chief observers can only view data that is accessible to a chief. 

  Condition:  `AuthenticationIdentity == "demo" && Member.StartsWith("chief-")`

* `member`

  Normal members have limited access to the member endpoints. 

  Condition: `!string.IsNullOrEmpty(AccountMember)`

* `member-observer`

  Member observers can only view data that is accessible to a member. 

  Condition:  `AuthenticationIdentity == "demo" && !string.IsNullOrEmpty(Member)`