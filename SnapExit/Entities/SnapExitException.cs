using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapExit.Entities;

// Causes minimal performance loss because it gets thrown in a cancelled Task
class SnapExitException : Exception;
