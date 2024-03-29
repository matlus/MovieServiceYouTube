﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DomainLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace MovieServiceYouTube.CustomActionResults;

[ExcludeFromCodeCoverage] //// This class is here for the sake of an example only
internal sealed class ExceptionActionResult : ActionResult
{
    private readonly Exception _exception;

    public ExceptionActionResult(Exception exception) => _exception = exception;

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        var httpResponse = context.HttpContext.Response!;

        httpResponse.Headers["Exception-Type"] = _exception.GetType().Name;

        if (_exception is MovieServiceBaseException movieServiceBaseException)
        {
            context.HttpContext.Features.Get<IHttpResponseFeature>()!.ReasonPhrase = movieServiceBaseException.Reason;
            httpResponse.StatusCode = MapExceptionToStatusCode(_exception);
        }

        await httpResponse.WriteAsync(_exception.Message);
    }

    private static int MapExceptionToStatusCode(Exception exception)
    {
        if (exception is MovieServiceNotFoundBaseException)
        {
            return 404;
        }
        else if (exception is MovieServiceBusinessBaseException)
        {
            return 400;
        }

        return 500;
    }
}