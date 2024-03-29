﻿// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//                         _oo0oo_
//                        o8888888o
//                        88" . "88
//                       (| - _ - |)
//                        0\  =  /0
//                      ___/`---'\___
//                    .' \\|        |// '.
//                   / \\|||    :   |||// \
//                  /  _|||||  -:-  |||||-\
//                 |   | \\\  -  /// |    |
//                 | \_|  ''\---/''  |_/  |
//                 \  .-\__  '-'  ___/-. /
//               ___'. .'  /--.--\  `. .'___
//            ."" '<  `.___\_<|>_/___.' >' "".
//           | | :  `- \`.;`\ _ /`;.`/ - ` : | |
//           \  \ `_.   \_ __\ /__ _/   .-` /  /
//       =====`-.____`.___ \_____/___.-`___.-'=====
//                         `=---='
// 
//       ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DotNetBrightener.Extensions.ProblemsResult.UI;

internal class UIEmbeddedResourcesReader : IUIResourcesReader
{
    private readonly Assembly _assembly;

    public UIEmbeddedResourcesReader(Assembly assembly)
    {
        _assembly = Guard.ThrowIfNull(assembly);
    }

    public IEnumerable<UIResource> UIResources
    {
        get
        {
            var embeddedResources = _assembly.GetManifestResourceNames();
            return ParseEmbeddedResources(embeddedResources);
        }
    }

    private IEnumerable<UIResource> ParseEmbeddedResources(string[] embeddedFiles)
    {
        const char SPLIT_SEPARATOR = '.';

        var resourceList = new List<UIResource>();

        foreach (var file in embeddedFiles)
        {
            var segments  = file.Split(SPLIT_SEPARATOR);
            var fileName  = segments[segments.Length - 2];
            var extension = segments[segments.Length - 1];

            using var contentStream = _assembly.GetManifestResourceStream(file)!;
            using var reader        = new StreamReader(contentStream);

            string result = reader.ReadToEnd();

            resourceList.Add(
                             UIResource.Create($"{fileName}.{extension}", result,
                                               ContentType.FromExtension(extension)));
        }

        return resourceList;
    }
}