using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace DiegoG.REST.ASPNET;

// I don't, really need to do any of this, do I? The server already knows everything it needs to know

//public class RESTObjectSerializerOutputFormatter : IOutputFormatter
//{
//    public bool CanWriteResult(OutputFormatterCanWriteContext context)
//    {
//        context.
//    }

//    public Task WriteAsync(OutputFormatterWriteContext context)
//    {
//        throw new NotImplementedException();
//    }
//}

//public class RESTObjectSerializerMvcOptions<TObjectCode> : IConfigureOptions<MvcOptions>
//    where TObjectCode : struct, IEquatable<TObjectCode>
//{
//    private readonly IRESTObjectSerializer<TObjectCode> Serializer;

//    public RESTObjectSerializerMvcOptions(IRESTObjectSerializer<TObjectCode> serializer)
//    {
//        Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
//    }

//    public void Configure(MvcOptions options)
//    {
//        var key = Serializer.TypeKey;
//        var mapping = options.FormatterMappings.GetMediaTypeMappingForFormat(key);
//        if (string.IsNullOrEmpty(mapping))
//        {
//            options.FormatterMappings.SetMediaTypeMappingForFormat(
//                key,
//                Serializer.MIMETypes[0]
//            );
//        }

//        var inputFormatter = new XmlSerializerInputFormatter(options);
//        inputFormatter.WrapperProviderFactories.Add(new ProblemDetailsWrapperProviderFactory());
//        options.InputFormatters.Add(inputFormatter);

//        var outputFormatter = new XmlSerializerOutputFormatter(_loggerFactory);
//        outputFormatter.WrapperProviderFactories.Add(new ProblemDetailsWrapperProviderFactory());
//        options.OutputFormatters.Add(outputFormatter);
//    }
//}
