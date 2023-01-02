# Roles

# Identity Roles

* `Admin`

  The administrator of the application has access to the identity endpoints. 

  Condition: `Identity == "admin"`

* `Default`

  Normal identities can sign in as a member. 

  Condition: `!string.IsNullOrEmpty(Identity)`

* `Secure`

  Secure identities can modify personal identity settings and the identity secret.

  Condition: `Identity != "demo"`

# Member Roles

* `Chief`

  The chief of a group has full access to the member endpoints. 

  Condition: `Member.StartsWith("chief-")`

* `Member`

  Normal members have limited access to the member endpoints. 

  Condition: `!string.IsNullOrEmpty(Member)`

* `Observer`

  Observers can only access HTTP GET endpoints. 

  Condition:  `Identity == "demo" && !string.IsNullOrEmpty(Member)`