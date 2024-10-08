global using MealBot.Auth.Common;
global using MealBot.Auth.Contracts;
global using MealBot.Auth.Contracts.Google;
global using MealBot.Auth.Contracts.Requests;
global using MealBot.Auth.Database;
global using MealBot.Auth.DomainErrors;
global using MealBot.Auth.Enums;
global using MealBot.Auth.Models;
global using MealBot.Auth.Options;
global using MealBot.Auth.Services;
global using MealBot.Common;

global using ErrorOr;

global using Google.Apis.Auth;

global using MediatR;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;

global using System.IdentityModel.Tokens.Jwt;
global using System.Net.Http.Json;
global using System.Security.Claims;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;