﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Output {
    public interface IOutputModuleDescriptor : IModuleDescriptor {
		int UpdateInterval { get; }
    }
}
