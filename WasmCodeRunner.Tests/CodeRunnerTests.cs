// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json.Linq;
using MLS.WasmCodeRunner;
using Xunit;

namespace MLS.WasmCodeRunner.Tests
{
    public class CodeRunnerTests
    {
        [Fact]
        public void It_can_run_code_and_return_result_with_correct_sequence_number()
        {
            using (var consoleState = new PreserveConsoleState())
            {
                var assembly = @"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mwAeAUABmwAeAWABmwAeAUIAaQBvAAMAAQAAAAYEmAAAAEMEnAACAAcAAwACAAkABQADAAoAGQADAAwAGwADAA4AHQADABAAHwADABIAIQADABQAIwADABYAJQAfAC4ABIAAAAEAAAAAAAAAAAAAAAAAzAAAAAQAAgABAAAAAAAAAHQA1AAAAAAABAABAAEAAAAAAAAAdABmAgAAAAAEAAIAAQAAAAAAAAB0ANECAAAAAAQAAQABAAAAAAAAAHQAvQAAAAAABAACAAEAAAAAAAAAdAChAwAAAAADAAIAKQBcAAAAADxjdXJyZW50PjVfXzEAPEZpYm9uYWNjaT5kX18xAElFbnVtZXJhYmxlYDEASUVudW1lcmF0b3JgMQBJbnQzMgA8bmV4dD41X18yADxNb2R1bGU+AFN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljAGdldF9DdXJyZW50TWFuYWdlZFRocmVhZElkADw+bF9faW5pdGlhbFRocmVhZElkAFRha2UASUVudW1lcmFibGUASURpc3Bvc2FibGUAU3lzdGVtLkNvbnNvbGUAY29uc29sZQBTeXN0ZW0uUnVudGltZQBXcml0ZUxpbmUAVHlwZQBTeXN0ZW0uSURpc3Bvc2FibGUuRGlzcG9zZQA8PjFfX3N0YXRlAENvbXBpbGVyR2VuZXJhdGVkQXR0cmlidXRlAERlYnVnZ2FibGVBdHRyaWJ1dGUAQXNzZW1ibHlUaXRsZUF0dHJpYnV0ZQBJdGVyYXRvclN0YXRlTWFjaGluZUF0dHJpYnV0ZQBEZWJ1Z2dlckhpZGRlbkF0dHJpYnV0ZQBBc3NlbWJseUZpbGVWZXJzaW9uQXR0cmlidXRlAEFzc2VtYmx5SW5mb3JtYXRpb25hbFZlcnNpb25BdHRyaWJ1dGUAQXNzZW1ibHlDb25maWd1cmF0aW9uQXR0cmlidXRlAENvbXBpbGF0aW9uUmVsYXhhdGlvbnNBdHRyaWJ1dGUAQXNzZW1ibHlQcm9kdWN0QXR0cmlidXRlAEFzc2VtYmx5Q29tcGFueUF0dHJpYnV0ZQBSdW50aW1lQ29tcGF0aWJpbGl0eUF0dHJpYnV0ZQBTeXN0ZW0uRGlhZ25vc3RpY3MuRGVidWcARmlib25hY2NpAGNvbnNvbGUuZGxsAFByb2dyYW0AU3lzdGVtAE1haW4AU3lzdGVtLlJlZmxlY3Rpb24ATm90U3VwcG9ydGVkRXhjZXB0aW9uAFN5c3RlbS5MaW5xAElFbnVtZXJhdG9yAFN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljLklFbnVtZXJhYmxlPFN5c3RlbS5JbnQzMj4uR2V0RW51bWVyYXRvcgBTeXN0ZW0uQ29sbGVjdGlvbnMuSUVudW1lcmFibGUuR2V0RW51bWVyYXRvcgAuY3RvcgBTeXN0ZW0uRGlhZ25vc3RpY3MAU3lzdGVtLlJ1bnRpbWUuQ29tcGlsZXJTZXJ2aWNlcwBEZWJ1Z2dpbmdNb2RlcwBTeXN0ZW0uUnVudGltZS5FeHRlbnNpb25zAFN5c3RlbS5Db2xsZWN0aW9ucwBPYmplY3QAU3lzdGVtLkNvbGxlY3Rpb25zLklFbnVtZXJhdG9yLlJlc2V0AEVudmlyb25tZW50AFN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljLklFbnVtZXJhdG9yPFN5c3RlbS5JbnQzMj4uQ3VycmVudABTeXN0ZW0uQ29sbGVjdGlvbnMuSUVudW1lcmF0b3IuQ3VycmVudABTeXN0ZW0uQ29sbGVjdGlvbnMuR2VuZXJpYy5JRW51bWVyYXRvcjxTeXN0ZW0uSW50MzI+LmdldF9DdXJyZW50AFN5c3RlbS5Db2xsZWN0aW9ucy5JRW51bWVyYXRvci5nZXRfQ3VycmVudAA8PjJfX2N1cnJlbnQATW92ZU5leHQAAAAAAPTh3kBkgaRGgtnPq+FiClEABCABAQgDIAABBSABARERBCABAQ4FIAEBEjUDIAACBRUSMQEIBCAAEwADIAAcBRUSPQEICCAAFRIxARMABCAAEkkIBwIVEjEBCAgQEAECFRI9AR4AFRI9AR4ACAMKAQgEAAEBCAMAAAgFBwMICAIEBwESDAiwP19/EdUKOgIGCAMAAAEHAAAVEj0BCAMgAAgHIAAVEjEBCAMoAAgDKAAcCAEACAAAAAAAHgEAAQBUAhZXcmFwTm9uRXhjZXB0aW9uVGhyb3dzAQgBAAcBAAAAAAwBAAdjb25zb2xlAAAKAQAFRGVidWcAAAwBAAcxLjAuMC4wAAAKAQAFMS4wLjAAABwBABdQcm9ncmFtKzxGaWJvbmFjY2k+ZF9fMQAABAEAAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAFQsAAAAAAAAAAAAAG4sAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAABgLAAAAAAAAAAAAAAAAF9Db3JFeGVNYWluAG1zY29yZWUuZGxsAAAAAAD/JQAgQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAMAAAAgDwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
                var runRequest = new WasmCodeRunnerRequest()
                {
                    Base64Assembly = assembly,
                    Succeeded = true
                };

                var theThing = new InteropMessage<WasmCodeRunnerRequest>(123, runRequest);

                var message = JObject.FromObject(theThing).ToString();

                var result = CodeRunner.ProcessRunRequest(message);

                result.Sequence.Should().Be(123);
                result.Data.CodeRunnerVersion.Should().NotBeNullOrEmpty();
                result.Data.Output.Select(o => o.Replace("\r\n", "\n")).Should().BeEquivalentTo(
                    "1", "1", "2", "3", "5", "8", "13", "21",
                    "34", "55", "89", "144", "233", "377", "610",
                    "987", "1597", "2584", "4181", "6765", "");

                consoleState.OutputIsRedirected.Should().BeFalse();
            }
        }

        [Fact]
        public void It_can_convert_diagnostics_into_output()
        {
            using (var consoleState = new PreserveConsoleState())
            {
                var diagnostic = new SerializableDiagnostic(0, 1, "message", 2, "stuff2");
                var runRequest = new WasmCodeRunnerRequest()
                {
                    Diagnostics = new[] { diagnostic },
                    Succeeded = false

                };

                var theThing = new InteropMessage<WasmCodeRunnerRequest>(123, runRequest);
                var message = JObject.FromObject(theThing).ToString();

                var result = CodeRunner.ProcessRunRequest(message);

                result.Sequence.Should().Be(123);
                result.Data.Output.Should().BeEquivalentTo("message");
                result.Data.Diagnostics.Should().BeNullOrEmpty();
                consoleState.OutputIsRedirected.Should().BeFalse();
            }
        }

        [Fact]
        public void It_can_run_a_Main_method_that_takes_string_array_forwarding_the_arg_to_running_code()
        {
            /* Compiled from 
            using System;
            using System.Collections.Generic;
            using System.Linq;
            namespace myApp
            { 
              class Program
              {
                  static void Main(string[] args)
                  {
                      Console.WriteLine(string.Join(", and then ", args.Reverse()));
                  }
              }
            }
            */
            using (var consoleState = new PreserveConsoleState())
            {
                var assembly = @"TVqQAAMAAAAEAAAA//8AALgAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAA4fug4AtAnNIbgBTM0hVGhpcyBwcm9ncmFtIGNhbm5vdCBiZSBydW4gaW4gRE9TIG1vZGUuDQ0KJAAAAAAAAABQRQAATAECAHBfK8YAAAAAAAAAAOAAIiALATAAAAgAAAACAAAAAAAAGiYAAAAgAAAAQAAAAAAAEAAgAAAAAgAABAAAAAAAAAAEAAAAAAAAAABgAAAAAgAAAAAAAAMAQIUAABAAABAAAAAAEAAAEAAAAAAAABAAAAAAAAAAAAAAAMglAABPAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAwAAACsJQAAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAACAAAAAAAAAAAAAAACCAAAEgAAAAAAAAAAAAAAC50ZXh0AAAAIAYAAAAgAAAACAAAAAIAAAAAAAAAAAAAAAAAACAAAGAucmVsb2MAAAwAAAAAQAAAAAIAAAAKAAAAAAAAAAAAAAAAAABAAABCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAD8JQAAAAAAAEgAAAACAAUAfCAAADAFAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGIAcgEAAHACKAEAACsoCwAACigMAAAKACoiAigNAAAKACoiAigNAAAKACoAQlNKQgEAAQAAAAAADAAAAHY0LjAuMzAzMTkAAAAABQBsAAAAtAEAACN+AAAgAgAANAIAACNTdHJpbmdzAAAAAFQEAAAcAAAAI1VTAHAEAAAQAAAAI0dVSUQAAACABAAAsAAAACNCbG9iAAAAAAAAAAIAAAFHFQAACQgAAAD6ATMAFgAAAQAAAA8AAAADAAAAAwAAAAEAAAANAAAACQAAAAEAAAABAAAAAQAAAAAAjQEBAAAAAAAGABYB9gEGAGgB9gEGAIkA4wEPABYCAAAGAE8BuQEGAPcAuQEGALQAuQEGANEAuQEGADYBuQEGAJ0AuQEGACoCqAEGAEYA0QEGAAEAHwAGAIYBqAEGAFEAqAEAAAAAFgAAAAAAAQABAAAAEACgAcsBLQABAAEAAQAQAA8AaAAtAAEAAwBQIAAAAACRAK8BQQABAGkgAAAAAIYY3QEGAAIAciAAAAAAhhjdAQYAAgAAAAEAJQIJAN0BAQARAN0BBgAZAN0BCgApAN0BEAAxAN0BEAA5AN0BEABBAN0BEABJAN0BEABRAN0BEABhAIEAFQBxALQBKQB5AHcAMwBZAN0BBgAuAAsARwAuABMAUAAuABsAbwAuACMAeAAuACsAjAAuADMAlwAuADsApAAuAEMAeAAuAEsAeAAEgAAAAQAAAAAAAAAAAAAAAABZAAAAAgAAAAAAAAAAAAAAOAA6AAAAAAAVACUAAAAAAABJRW51bWVyYWJsZWAxAENsYXNzMQA8TW9kdWxlPgBTeXN0ZW0uQ29sbGVjdGlvbnMuR2VuZXJpYwBuZXRzdGFuZGFyZABFbnVtZXJhYmxlAENvbnNvbGUAYmxhem9yLWNvbnNvbGUAYmxhem9yX2NvbnNvbGUAV3JpdGVMaW5lAFJldmVyc2UARGVidWdnYWJsZUF0dHJpYnV0ZQBBc3NlbWJseVRpdGxlQXR0cmlidXRlAEFzc2VtYmx5RmlsZVZlcnNpb25BdHRyaWJ1dGUAQXNzZW1ibHlJbmZvcm1hdGlvbmFsVmVyc2lvbkF0dHJpYnV0ZQBBc3NlbWJseUNvbmZpZ3VyYXRpb25BdHRyaWJ1dGUAQ29tcGlsYXRpb25SZWxheGF0aW9uc0F0dHJpYnV0ZQBBc3NlbWJseVByb2R1Y3RBdHRyaWJ1dGUAQXNzZW1ibHlDb21wYW55QXR0cmlidXRlAFJ1bnRpbWVDb21wYXRpYmlsaXR5QXR0cmlidXRlAFN0cmluZwBibGF6b3ItY29uc29sZS5kbGwAUHJvZ3JhbQBTeXN0ZW0ATWFpbgBKb2luAFN5c3RlbS5SZWZsZWN0aW9uAG15QXBwAFN5c3RlbS5MaW5xAC5jdG9yAFN5c3RlbS5EaWFnbm9zdGljcwBTeXN0ZW0uUnVudGltZS5Db21waWxlclNlcnZpY2VzAERlYnVnZ2luZ01vZGVzAGFyZ3MAT2JqZWN0AAAAAAAXLAAgAGEAbgBkACAAdABoAGUAbgAgAAAAAAACgsWqj4aEQ6y2ufPiuiO1AAQgAQEIAyAAAQUgAQEREQQgAQEODxABARUSNQEeABUSNQEeAAMKAQ4JAAIODhUSNQEOBAABAQ4IzHsT/80t3VEFAAEBHQ4IAQAIAAAAAAAeAQABAFQCFldyYXBOb25FeGNlcHRpb25UaHJvd3MBCAEABwEAAAAAEwEADmJsYXpvci1jb25zb2xlAAAKAQAFRGVidWcAAAwBAAcxLjAuMC4wAAAKAQAFMS4wLjAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAA8CUAAAAAAAAAAAAACiYAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAPwlAAAAAAAAAAAAAAAAX0NvckRsbE1haW4AbXNjb3JlZS5kbGwAAAAAAP8lACAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAMAAAAHDYAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
                var runRequest = new WasmCodeRunnerRequest()
                {
                    Base64Assembly = assembly,
                    Succeeded = true,
                    RunArgs = "first second \"third argument\""
                };

                var theThing = new InteropMessage<WasmCodeRunnerRequest>(123, runRequest);
                var message = JObject.FromObject(theThing).ToString();

                var result = CodeRunner.ProcessRunRequest(message);

                result.Sequence.Should().Be(123);
                result.Data.Output.Select(o => o.Trim()).Should().BeEquivalentTo("third argument, and then second, and then first", "");
                consoleState.OutputIsRedirected.Should().BeFalse();
            }
        }

        [Fact]
        public void It_can_run_a_Main_method_that_requires_argument_bindings()
        {
            /* Compiled from 
            using System;
            using System.Collections.Generic;
            using System.Linq;
            namespace myApp
            { 
              class Program
              {
                  static void Main(string region, string session)
                  {
                      Console.WriteLine($"region {region} session {session}");
                  }
              }
            }
            */
            using (var consoleState = new PreserveConsoleState())
            {
                var assembly = @"TVqQAAMAAAAEAAAA//8AALgAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAA4fug4AtAnNIbgBTM0hVGhpcyBwcm9ncmFtIGNhbm5vdCBiZSBydW4gaW4gRE9TIG1vZGUuDQ0KJAAAAAAAAABQRQAATAECAMGGOMkAAAAAAAAAAOAAIiALATAAAAYAAAACAAAAAAAAwiUAAAAgAAAAQAAAAAAAEAAgAAAAAgAABAAAAAAAAAAEAAAAAAAAAABgAAAAAgAAAAAAAAMAQIUAABAAABAAAAAAEAAAEAAAAAAAABAAAAAAAAAAAAAAAHAlAABPAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAwAAABUJQAAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAACAAAAAAAAAAAAAAACCAAAEgAAAAAAAAAAAAAAC50ZXh0AAAAyAUAAAAgAAAABgAAAAIAAAAAAAAAAAAAAAAAACAAAGAucmVsb2MAAAwAAAAAQAAAAAIAAAAIAAAAAAAAAAAAAAAAAABAAABCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACkJQAAAAAAAEgAAAACAAUAeCAAANwEAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFIAcgEAAHACAygKAAAKKAsAAAoAKiICKAwAAAoAKiICKAwAAAoAKgBCU0pCAQABAAAAAAAMAAAAdjQuMC4zMDMxOQAAAAAFAGwAAACgAQAAI34AAAwCAAD4AQAAI1N0cmluZ3MAAAAABAQAADAAAAAjVVMANAQAABAAAAAjR1VJRAAAAEQEAACYAAAAI0Jsb2IAAAAAAAAAAgAAAUcVAAAJAAAAAPoBMwAWAAABAAAADQAAAAMAAAADAAAAAgAAAAwAAAAJAAAAAQAAAAEAAAAAAFEBAQAAAAAABgDaALgBBgAsAbgBBgBNAKUBDwDYAQAABgATAYcBBgC7AIcBBgB4AIcBBgCVAIcBBgD6AIcBBgBhAIcBBgDuAWwBBgBKAWwBBgAdAGwBAAAAAAgAAAAAAAEAAQAAABAAZAGZAS0AAQABAAEAEAABADQALQABAAMAUCAAAAAAkQBzASoAAQBlIAAAAACGGJ8BBgADAG4gAAAAAIYYnwEGAAMAAAABAHgBAAACAH8BCQCfAQEAEQCfAQYAGQCfAQoAKQCfARAAMQCfARAAOQCfARAAQQCfARAASQCfARAAUQCfARAAYQDnARUAaQBDABwAWQCfAQYALgALADAALgATADkALgAbAFgALgAjAGEALgArAHUALgAzAIAALgA7AI0ALgBDAGEALgBLAGEABIAAAAEAAAAAAAAAAAAAAAAAJQAAAAIAAAAAAAAAAAAAACEAEQAAAAAAAAAAAABDbGFzczEAPE1vZHVsZT4AbmV0c3RhbmRhcmQAQ29uc29sZQBibGF6b3ItY29uc29sZQBibGF6b3JfY29uc29sZQBXcml0ZUxpbmUARGVidWdnYWJsZUF0dHJpYnV0ZQBBc3NlbWJseVRpdGxlQXR0cmlidXRlAEFzc2VtYmx5RmlsZVZlcnNpb25BdHRyaWJ1dGUAQXNzZW1ibHlJbmZvcm1hdGlvbmFsVmVyc2lvbkF0dHJpYnV0ZQBBc3NlbWJseUNvbmZpZ3VyYXRpb25BdHRyaWJ1dGUAQ29tcGlsYXRpb25SZWxheGF0aW9uc0F0dHJpYnV0ZQBBc3NlbWJseVByb2R1Y3RBdHRyaWJ1dGUAQXNzZW1ibHlDb21wYW55QXR0cmlidXRlAFJ1bnRpbWVDb21wYXRpYmlsaXR5QXR0cmlidXRlAFN0cmluZwBibGF6b3ItY29uc29sZS5kbGwAUHJvZ3JhbQBTeXN0ZW0ATWFpbgByZWdpb24Ac2Vzc2lvbgBTeXN0ZW0uUmVmbGVjdGlvbgBteUFwcAAuY3RvcgBTeXN0ZW0uRGlhZ25vc3RpY3MAU3lzdGVtLlJ1bnRpbWUuQ29tcGlsZXJTZXJ2aWNlcwBEZWJ1Z2dpbmdNb2RlcwBGb3JtYXQAT2JqZWN0AAAAAAAtcgBlAGcAaQBvAG4AIAB7ADAAfQAgAHMAZQBzAHMAaQBvAG4AIAB7ADEAfQAAAMkIVlAJa05LsS5GWGEy65QABCABAQgDIAABBSABARERBCABAQ4GAAMODhwcBAABAQ4IzHsT/80t3VEFAAIBDg4IAQAIAAAAAAAeAQABAFQCFldyYXBOb25FeGNlcHRpb25UaHJvd3MBCAEABwEAAAAAEwEADmJsYXpvci1jb25zb2xlAAAKAQAFRGVidWcAAAwBAAcxLjAuMC4wAAAKAQAFMS4wLjAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAACYJQAAAAAAAAAAAACyJQAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAApCUAAAAAAAAAAAAAAABfQ29yRGxsTWFpbgBtc2NvcmVlLmRsbAAAAAAA/yUAIAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAADAAAAMQ1AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";
                var runRequest = new WasmCodeRunnerRequest()
                {
                    Base64Assembly = assembly,
                    Succeeded = true,
                    RunArgs = "--region region1 --session sessionOne"
                };

                var theThing = new InteropMessage<WasmCodeRunnerRequest>(123, runRequest);
                var message = JObject.FromObject(theThing).ToString();

                var result = CodeRunner.ProcessRunRequest(message);

                result.Sequence.Should().Be(123);
                result.Data.Output.Select(o => o.Trim()).Should().BeEquivalentTo("region region1 session sessionOne", "");
                consoleState.OutputIsRedirected.Should().BeFalse();
            }
        }

        [Fact]
        public void It_can_run_a_Main_method_with_missing_arguments()
        {
            /* Compiled form 
                        using System;
                        using System.Collections.Generic;
                        using System.Linq;
            namespace myApp
                {
                    class Program
                    {
                        static void Main(string region)
                        {
                            Console.WriteLine($"region {region}");
                        }
                    }
                }
            */
            using (var consoleState = new PreserveConsoleState())
            {
                var assembly = "TVqQAAMAAAAEAAAA//8AALgAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAA4fug4AtAnNIbgBTM0hVGhpcyBwcm9ncmFtIGNhbm5vdCBiZSBydW4gaW4gRE9TIG1vZGUuDQ0KJAAAAAAAAABQRQAATAECAKUKHa4AAAAAAAAAAOAAIiALATAAAAYAAAACAAAAAAAAliUAAAAgAAAAQAAAAAAAEAAgAAAAAgAABAAAAAAAAAAEAAAAAAAAAABgAAAAAgAAAAAAAAMAQIUAABAAABAAAAAAEAAAEAAAAAAAABAAAAAAAAAAAAAAAEQlAABPAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAwAAAAoJQAAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAACAAAAAAAAAAAAAAACCAAAEgAAAAAAAAAAAAAAC50ZXh0AAAAnAUAAAAgAAAABgAAAAIAAAAAAAAAAAAAAAAAACAAAGAucmVsb2MAAAwAAAAAQAAAAAIAAAAIAAAAAAAAAAAAAAAAAABAAABCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAB4JQAAAAAAAEgAAAACAAUAeCAAALAEAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAE4AcgEAAHACKAoAAAooCwAACgAqIgIoDAAACgAqIgIoDAAACgAqAABCU0pCAQABAAAAAAAMAAAAdjQuMC4zMDMxOQAAAAAFAGwAAACYAQAAI34AAAQCAADwAQAAI1N0cmluZ3MAAAAA9AMAABgAAAAjVVMADAQAABAAAAAjR1VJRAAAABwEAACUAAAAI0Jsb2IAAAAAAAAAAgAAAUcVAAAJAAAAAPoBMwAWAAABAAAADQAAAAMAAAADAAAAAQAAAAwAAAAJAAAAAQAAAAEAAAAAAFEBAQAAAAAABgDaALABBgAsAbABBgBNAJ0BDwDQAQAABgATAX8BBgC7AH8BBgB4AH8BBgCVAH8BBgD6AH8BBgBhAH8BBgDmAWwBBgBKAWwBBgAdAGwBAAAAAAgAAAAAAAEAAQAAABAAZAGRAS0AAQABAAEAEAABADQALQABAAMAUCAAAAAAkQBzARsAAQBkIAAAAACGGJcBBgACAG0gAAAAAIYYlwEGAAIAAAABAHgBCQCXAQEAEQCXAQYAGQCXAQoAKQCXARAAMQCXARAAOQCXARAAQQCXARAASQCXARAAUQCXARAAYQDfARUAaQBDABsAWQCXAQYALgALACkALgATADIALgAbAFEALgAjAFoALgArAG4ALgAzAHkALgA7AIYALgBDAFoALgBLAFoABIAAAAEAAAAAAAAAAAAAAAAAJQAAAAIAAAAAAAAAAAAAACAAEQAAAAAAAAAAQ2xhc3MxADxNb2R1bGU+AG5ldHN0YW5kYXJkAENvbnNvbGUAYmxhem9yLWNvbnNvbGUAYmxhem9yX2NvbnNvbGUAV3JpdGVMaW5lAERlYnVnZ2FibGVBdHRyaWJ1dGUAQXNzZW1ibHlUaXRsZUF0dHJpYnV0ZQBBc3NlbWJseUZpbGVWZXJzaW9uQXR0cmlidXRlAEFzc2VtYmx5SW5mb3JtYXRpb25hbFZlcnNpb25BdHRyaWJ1dGUAQXNzZW1ibHlDb25maWd1cmF0aW9uQXR0cmlidXRlAENvbXBpbGF0aW9uUmVsYXhhdGlvbnNBdHRyaWJ1dGUAQXNzZW1ibHlQcm9kdWN0QXR0cmlidXRlAEFzc2VtYmx5Q29tcGFueUF0dHJpYnV0ZQBSdW50aW1lQ29tcGF0aWJpbGl0eUF0dHJpYnV0ZQBTdHJpbmcAYmxhem9yLWNvbnNvbGUuZGxsAFByb2dyYW0AU3lzdGVtAE1haW4AcmVnaW9uAFN5c3RlbS5SZWZsZWN0aW9uAG15QXBwAC5jdG9yAFN5c3RlbS5EaWFnbm9zdGljcwBTeXN0ZW0uUnVudGltZS5Db21waWxlclNlcnZpY2VzAERlYnVnZ2luZ01vZGVzAEZvcm1hdABPYmplY3QAAAAAABVyAGUAZwBpAG8AbgAgAHsAMAB9AAAApmTY9+J6lUu2xoh6A4AHkgAEIAEBCAMgAAEFIAEBEREEIAEBDgUAAg4OHAQAAQEOCMx7E//NLd1RCAEACAAAAAAAHgEAAQBUAhZXcmFwTm9uRXhjZXB0aW9uVGhyb3dzAQgBAAcBAAAAABMBAA5ibGF6b3ItY29uc29sZQAACgEABURlYnVnAAAMAQAHMS4wLjAuMAAACgEABTEuMC4wAAAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAbCUAAAAAAAAAAAAAhiUAAAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHglAAAAAAAAAAAAAAAAX0NvckRsbE1haW4AbXNjb3JlZS5kbGwAAAAAAP8lACAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAADAAAAJg1AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";
                var runRequest = new WasmCodeRunnerRequest()
                {
                    Base64Assembly = assembly,
                    Succeeded = true,
                    RunArgs = "--region region1 --session sessionOne"
                };

                var theThing = new InteropMessage<WasmCodeRunnerRequest>(123, runRequest);
                var message = JObject.FromObject(theThing).ToString();

                var result = CodeRunner.ProcessRunRequest(message);

                result.Sequence.Should().Be(123);
                result.Data.Output.Select(o => o.Trim()).Should().BeEquivalentTo("region region1", "");
                consoleState.OutputIsRedirected.Should().BeFalse();
            }
        }

        [Fact]
        public void It_can_run_a_Main_method_that_takes_string_array_with_dashdash_arguments()
        {
            /* Compiled from
             * 
                        using System;
                        using System.Collections.Generic;
                        using System.Linq;
            namespace myApp
                {
                    class Program
                    {
                        static void Main(string[] args)
                        {
                            Console.WriteLine(string.Join("" "", args.Reverse()));
                        }
                    }
                }
            */
            using (var consoleState = new PreserveConsoleState())
            {
                var assembly = "TVqQAAMAAAAEAAAA//8AALgAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAA4fug4AtAnNIbgBTM0hVGhpcyBwcm9ncmFtIGNhbm5vdCBiZSBydW4gaW4gRE9TIG1vZGUuDQ0KJAAAAAAAAABQRQAATAECALX0yL0AAAAAAAAAAOAAIiALATAAAAgAAAACAAAAAAAABiYAAAAgAAAAQAAAAAAAEAAgAAAAAgAABAAAAAAAAAAEAAAAAAAAAABgAAAAAgAAAAAAAAMAQIUAABAAABAAAAAAEAAAEAAAAAAAABAAAAAAAAAAAAAAALQlAABPAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAwAAACYJQAAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAACAAAAAAAAAAAAAAACCAAAEgAAAAAAAAAAAAAAC50ZXh0AAAADAYAAAAgAAAACAAAAAIAAAAAAAAAAAAAAAAAACAAAGAucmVsb2MAAAwAAAAAQAAAAAIAAAAKAAAAAAAAAAAAAAAAAABAAABCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADoJQAAAAAAAEgAAAACAAUAfCAAABwFAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGIAcgEAAHACKAEAACsoCwAACigMAAAKACoiAigNAAAKACoiAigNAAAKACoAQlNKQgEAAQAAAAAADAAAAHY0LjAuMzAzMTkAAAAABQBsAAAAtAEAACN+AAAgAgAANAIAACNTdHJpbmdzAAAAAFQEAAAIAAAAI1VTAFwEAAAQAAAAI0dVSUQAAABsBAAAsAAAACNCbG9iAAAAAAAAAAIAAAFHFQAACQgAAAD6ATMAFgAAAQAAAA8AAAADAAAAAwAAAAEAAAANAAAACQAAAAEAAAABAAAAAQAAAAAAjQEBAAAAAAAGABYB9gEGAGgB9gEGAIkA4wEPABYCAAAGAE8BuQEGAPcAuQEGALQAuQEGANEAuQEGADYBuQEGAJ0AuQEGACoCqAEGAEYA0QEGAAEAHwAGAIYBqAEGAFEAqAEAAAAAFgAAAAAAAQABAAAAEACgAcsBLQABAAEAAQAQAA8AaAAtAAEAAwBQIAAAAACRAK8BQQABAGkgAAAAAIYY3QEGAAIAciAAAAAAhhjdAQYAAgAAAAEAJQIJAN0BAQARAN0BBgAZAN0BCgApAN0BEAAxAN0BEAA5AN0BEABBAN0BEABJAN0BEABRAN0BEABhAIEAFQBxALQBKQB5AHcAMwBZAN0BBgAuAAsARwAuABMAUAAuABsAbwAuACMAeAAuACsAjAAuADMAlwAuADsApAAuAEMAeAAuAEsAeAAEgAAAAQAAAAAAAAAAAAAAAABZAAAAAgAAAAAAAAAAAAAAOAA6AAAAAAAVACUAAAAAAABJRW51bWVyYWJsZWAxAENsYXNzMQA8TW9kdWxlPgBTeXN0ZW0uQ29sbGVjdGlvbnMuR2VuZXJpYwBuZXRzdGFuZGFyZABFbnVtZXJhYmxlAENvbnNvbGUAYmxhem9yLWNvbnNvbGUAYmxhem9yX2NvbnNvbGUAV3JpdGVMaW5lAFJldmVyc2UARGVidWdnYWJsZUF0dHJpYnV0ZQBBc3NlbWJseVRpdGxlQXR0cmlidXRlAEFzc2VtYmx5RmlsZVZlcnNpb25BdHRyaWJ1dGUAQXNzZW1ibHlJbmZvcm1hdGlvbmFsVmVyc2lvbkF0dHJpYnV0ZQBBc3NlbWJseUNvbmZpZ3VyYXRpb25BdHRyaWJ1dGUAQ29tcGlsYXRpb25SZWxheGF0aW9uc0F0dHJpYnV0ZQBBc3NlbWJseVByb2R1Y3RBdHRyaWJ1dGUAQXNzZW1ibHlDb21wYW55QXR0cmlidXRlAFJ1bnRpbWVDb21wYXRpYmlsaXR5QXR0cmlidXRlAFN0cmluZwBibGF6b3ItY29uc29sZS5kbGwAUHJvZ3JhbQBTeXN0ZW0ATWFpbgBKb2luAFN5c3RlbS5SZWZsZWN0aW9uAG15QXBwAFN5c3RlbS5MaW5xAC5jdG9yAFN5c3RlbS5EaWFnbm9zdGljcwBTeXN0ZW0uUnVudGltZS5Db21waWxlclNlcnZpY2VzAERlYnVnZ2luZ01vZGVzAGFyZ3MAT2JqZWN0AAAAAAADIAAAAAAAOtumbg3G9kKRlPub5ADIcgAEIAEBCAMgAAEFIAEBEREEIAEBDg8QAQEVEjUBHgAVEjUBHgADCgEOCQACDg4VEjUBDgQAAQEOCMx7E//NLd1RBQABAR0OCAEACAAAAAAAHgEAAQBUAhZXcmFwTm9uRXhjZXB0aW9uVGhyb3dzAQgBAAcBAAAAABMBAA5ibGF6b3ItY29uc29sZQAACgEABURlYnVnAAAMAQAHMS4wLjAuMAAACgEABTEuMC4wAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAANwlAAAAAAAAAAAAAPYlAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAADoJQAAAAAAAAAAAAAAAF9Db3JEbGxNYWluAG1zY29yZWUuZGxsAAAAAAD/JQAgABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAMAAAACDYAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
                var runRequest = new WasmCodeRunnerRequest()
                {
                    Base64Assembly = assembly,
                    Succeeded = true,
                    RunArgs = "--region region1 --session sessionOne"
                };

                var theThing = new InteropMessage<WasmCodeRunnerRequest>(123, runRequest);
                var message = JObject.FromObject(theThing).ToString();

                var result = CodeRunner.ProcessRunRequest(message);

                result.Sequence.Should().Be(123);
                result.Data.Output.Select(o => o.Trim()).Should().BeEquivalentTo("sessionOne --session region1 --region", "");
                consoleState.OutputIsRedirected.Should().BeFalse();
            }
        }

        [Fact]
        public void It_can_run_a_private_main_method()
        {
            /* Compiled from 
using System;
namespace myApp
{ 
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello World!");
        }
    }
}
*/
            using (var consoleState = new PreserveConsoleState())
            {
                var assembly = @"TVqQAAMAAAAEAAAA//8AALgAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAA4fug4AtAnNIbgBTM0hVGhpcyBwcm9ncmFtIGNhbm5vdCBiZSBydW4gaW4gRE9TIG1vZGUuDQ0KJAAAAAAAAABQRQAATAECAG+WAqcAAAAAAAAAAOAAIiALATAAAAYAAAACAAAAAAAAYiUAAAAgAAAAQAAAAAAAEAAgAAAAAgAABAAAAAAAAAAEAAAAAAAAAABgAAAAAgAAAAAAAAMAQIUAABAAABAAAAAAEAAAEAAAAAAAABAAAAAAAAAAAAAAABAlAABPAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAwAAAD0JAAAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAACAAAAAAAAAAAAAAACCAAAEgAAAAAAAAAAAAAAC50ZXh0AAAAaAUAAAAgAAAABgAAAAIAAAAAAAAAAAAAAAAAACAAAGAucmVsb2MAAAwAAAAAQAAAAAIAAAAIAAAAAAAAAAAAAAAAAABAAABCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABEJQAAAAAAAEgAAAACAAUAcCAAAIQEAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADYAcgEAAHAoCgAACgAqIgIoCwAACgAqIgIoCwAACgAqQlNKQgEAAQAAAAAADAAAAHY0LjAuMzAzMTkAAAAABQBsAAAAhAEAACN+AADwAQAA2AEAACNTdHJpbmdzAAAAAMgDAAAcAAAAI1VTAOQDAAAQAAAAI0dVSUQAAAD0AwAAkAAAACNCbG9iAAAAAAAAAAIAAAFHFAAACQAAAAD6ATMAFgAAAQAAAAwAAAADAAAAAwAAAAsAAAAJAAAAAQAAAAEAAAAAAEoBAQAAAAAABgDaAKIBBgAsAaIBBgBNAI8BDwDCAQAABgATAXEBBgC7AHEBBgB4AHEBBgCVAHEBBgD6AHEBBgBhAHEBBgDRAWUBBgAdAGUBAAAAAAgAAAAAAAEAAQAAABAAXQGDAS0AAQABAAEAEAABADQALQABAAMAUCAAAAAAkQBsASMAAQBeIAAAAACGGIkBBgABAGcgAAAAAIYYiQEGAAEACQCJAQEAEQCJAQYAGQCJAQoAKQCJARAAMQCJARAAOQCJARAAQQCJARAASQCJARAAUQCJARAAYQBDABUAWQCJAQYALgALACcALgATADAALgAbAE8ALgAjAFgALgArAGwALgAzAHcALgA7AIQALgBDAFgALgBLAFgABIAAAAEAAAAAAAAAAAAAAAAAJQAAAAIAAAAAAAAAAAAAABoAEQAAAAAAAAAAAABDbGFzczEAPE1vZHVsZT4AbmV0c3RhbmRhcmQAQ29uc29sZQBibGF6b3ItY29uc29sZQBibGF6b3JfY29uc29sZQBXcml0ZUxpbmUARGVidWdnYWJsZUF0dHJpYnV0ZQBBc3NlbWJseVRpdGxlQXR0cmlidXRlAEFzc2VtYmx5RmlsZVZlcnNpb25BdHRyaWJ1dGUAQXNzZW1ibHlJbmZvcm1hdGlvbmFsVmVyc2lvbkF0dHJpYnV0ZQBBc3NlbWJseUNvbmZpZ3VyYXRpb25BdHRyaWJ1dGUAQ29tcGlsYXRpb25SZWxheGF0aW9uc0F0dHJpYnV0ZQBBc3NlbWJseVByb2R1Y3RBdHRyaWJ1dGUAQXNzZW1ibHlDb21wYW55QXR0cmlidXRlAFJ1bnRpbWVDb21wYXRpYmlsaXR5QXR0cmlidXRlAGJsYXpvci1jb25zb2xlLmRsbABQcm9ncmFtAFN5c3RlbQBNYWluAFN5c3RlbS5SZWZsZWN0aW9uAG15QXBwAC5jdG9yAFN5c3RlbS5EaWFnbm9zdGljcwBTeXN0ZW0uUnVudGltZS5Db21waWxlclNlcnZpY2VzAERlYnVnZ2luZ01vZGVzAE9iamVjdAAAGUgAZQBsAGwAbwAgAFcAbwByAGwAZAAhAAAAXQ9GanmtTEuzgc0NDhYR8wAEIAEBCAMgAAEFIAEBEREEIAEBDgQAAQEOCMx7E//NLd1RAwAAAQgBAAgAAAAAAB4BAAEAVAIWV3JhcE5vbkV4Y2VwdGlvblRocm93cwEIAQAHAQAAAAATAQAOYmxhem9yLWNvbnNvbGUAAAoBAAVEZWJ1ZwAADAEABzEuMC4wLjAAAAoBAAUxLjAuMAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAA4JQAAAAAAAAAAAABSJQAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAARCUAAAAAAAAAAAAAAABfQ29yRGxsTWFpbgBtc2NvcmVlLmRsbAAAAAAA/yUAIAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAADAAAAGQ1AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";
                var runRequest = new WasmCodeRunnerRequest()
                {
                    Base64Assembly = assembly,
                    Succeeded = true
                };

                var theThing = new InteropMessage<WasmCodeRunnerRequest>(123, runRequest);
                var message = JObject.FromObject(theThing).ToString();

                var result = CodeRunner.ProcessRunRequest(message);

                result.Sequence.Should().Be(123);
                result.Data.Output.Select(o => o.Trim()).Should().BeEquivalentTo("Hello World!", "");
                consoleState.OutputIsRedirected.Should().BeFalse();
            }
        }

        [Fact]
        public void It_provides_errors_for_missing_type()
        {
            using (var consoleState = new PreserveConsoleState())
            {
                var referencedCompilation = Compile(@"
public class C
{ 
}");

                var text = @"
class D
{
    public static void Main()
    {
        C c= new C();
    }
}";
                var other = Compile(text, referencedCompilation.ToMetadataReference());


                using (var stream = new MemoryStream())
                {
                    other.Emit(peStream: stream);
                    var encodedAssembly = System.Convert.ToBase64String(stream.ToArray());

                    var runRequest = new WasmCodeRunnerRequest()
                    {
                        Succeeded = true,
                        Base64Assembly = encodedAssembly
                    };

                    var response = CodeRunner.ExecuteRunRequest(runRequest, 1);

                    response.Data.RunnerException.Should().Match("*Missing type `C`*");
                    consoleState.OutputIsRedirected.Should().BeFalse();
                }
            }
        }

        [Fact]
        public void It_returns_null_if_there_is_no_assembly_or_diagnostics()
        {
            using (var consoleState = new PreserveConsoleState())
            {
                var runRequest = new WasmCodeRunnerRequest();
                var interopMessage = new InteropMessage<WasmCodeRunnerRequest>(0, runRequest);
                var text = JObject.FromObject(interopMessage).ToString();
                CodeRunner.ProcessRunRequest(text).Should().Be(null);
                consoleState.OutputIsRedirected.Should().BeFalse();
            }
        }


        [Fact]
        public void It_returns_an_error_if_there_is_no_entry_point()
        {
            using (var consoleState = new PreserveConsoleState())
            {
                var compilation = Compile("class C {}");

                using (var stream = new MemoryStream())
                {
                    compilation.Emit(peStream: stream);
                    var encodedAssembly = Convert.ToBase64String(stream.ToArray());

                    var runRequest = new WasmCodeRunnerRequest()
                    {
                        Succeeded = true,
                        Base64Assembly = encodedAssembly
                    };

                    var output = CodeRunner.ExecuteRunRequest(runRequest, 1).Data.Output;
                    output.Single().Should().Be("error CS5001: Program does not contain a static 'Main' method suitable for an entry point");
                    consoleState.OutputIsRedirected.Should().BeFalse();
                }
            }
        }

        private static string CompileAndEncode(string text, params MetadataReference[] additionalReferences)
        {
            var compilation = Compile(text);

            using (var stream = new MemoryStream())
            {
                compilation.Emit(peStream: stream);
                var encodedAssembly = Convert.ToBase64String(stream.ToArray());

                return encodedAssembly;
            }
        }

        private static Compilation Compile(string text, params MetadataReference[] additionalReferences)
        {
            var refs = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(decimal).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
            }.Concat(additionalReferences);

            return CSharpCompilation.Create("assembly.dll", new[] { CSharpSyntaxTree.ParseText(text) }, refs,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }
    }
}
