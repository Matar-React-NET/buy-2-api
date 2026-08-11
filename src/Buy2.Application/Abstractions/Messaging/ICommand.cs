using System;
using System.Collections.Generic;
using System.Text;

namespace Buy2.Application.Abstractions.Messaging;

public interface ICommand;
public interface ICommand<out TResponse> : ICommand;
