﻿using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMemberManager
{
    Member Get(string uniqueName);

    IEnumerable<Member> GetAll();
}