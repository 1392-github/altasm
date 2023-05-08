using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AltASM
{
    public class Program
    {
        static string comp_v1(string[] origin)
        {
            string out_str = "";
            bool original_asm = false;
            foreach (string line in origin)
            {
                if (original_asm)
                {
                    if (new Regex("^ *} *$").IsMatch(line))
                    {
                        original_asm = false;
                    }
                    else
                    {
                        out_str += line;
                        out_str += "\n";
                    }
                }
                // = 연산자
                if (new Regex("^[0-9A-Fa-f]{1,6} {0,}= {0,}[0-9A-Fa-f]{1,2}$").IsMatch(line)) // 앞에 0~9, A~F, a~f 1~6자, 스페이즈 0자 이상, 뒤에도 0~9, A~F, a~f 1~2자
                {
                    string[] both = line.Split("="); // = 부호 기준으로 자름
                    both = both.Select(x => x.Trim()).ToArray(); // 공백 문자를 제거해 2개의 주소로 만듬
                    out_str += $"lda #${both[1]}\n";
                    out_str += $"sta ${both[0]}\n";
                }
                if (new Regex("^[0-9A-Fa-f]{1,6} {0,}= {0,}\\$[0-9A-Fa-f]{1,6}$").IsMatch(line))
                {
                    string[] both = line.Split("=");
                    both = both.Select(x => x.Trim().Replace("$", "")).ToArray();
                    out_str += $"lda ${both[1]}\n";
                    out_str += $"sta ${both[0]}\n";
                }
                if (new Regex("^reg_a {0,}= {0,}[0-9A-Fa-f]{1,2}$").IsMatch(line))
                {
                    string[] both = line.Split("=");
                    both = both.Select(x => x.Trim().Replace("$", "")).ToArray();
                    out_str += $"lda ${both[1]}\n";
                }
                if (IsMatch("[0-9A-Fa-f]{1,6} {0,}= {0,}reg_a", line))
                {
                    string[] both = line.Split("=");
                    both = both.Select(x => x.Trim().Replace("$", "")).ToArray();
                    out_str += $"sta #${both[0]}\n";
                }
                if (new Regex("^asm[ \n]*{ *$").IsMatch(line))
                {
                    original_asm = true;
                }
                if (IsMatch("$([1-9A-Fa-f]{0,6}) *= *reg_a$", line))
                {
                    out_str += $"sta ${new Regex("$([1-9A-Fa-f]{0,6}) *= *reg_a^").Match(line).Groups[1].Value}\n";
                }
                if (line == "return")
                {
                    out_str += "rtl\n";
                }
// new Regex
            }
            return out_str;
        }
        static string[,] errors =
        {
            {
                "AltASM 버전 {0}은 지원하지 않는 AltASM 버전입니다",
                "1 - 최신 버전의 AltASM 컴파일러를 사용해 보세요\n" +
                "2 - #version 지시문이 잘못되었는지 확인해 보세요"
            },
            {
                "#version 지시문에 1보다 작은 숫자를 넣었습니다",
                "#version 지시문의 버전에 1보다 큰 숫자를 넣으세요"
            },
            {
                "이 기능은 초안으로, 현재 구현되지 않았습니다",
                "1 - 최신 AltASM 컴파일러로 업그레이드 하세요\n" +
                "2 - #version 문을 수정하세요"
            }
        };
        static string[,] warnings =
        {

        };
        static int max_version = 1; // 지원하는 AltASM 최신 버전
        static void PrintError(int error_code, string problem, params string[] extra_info)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"[ERR {error_code}] ");
            Console.WriteLine(errors[error_code, 0], extra_info);
            Console.WriteLine("문제가 발생한 구문 :");
            Console.WriteLine(problem);
            Console.WriteLine("해결 방법 : ");
            Console.WriteLine(errors[error_code, 1]);
            Console.ResetColor();
        }
        static bool IsMatch(string reg, string value)
        {
            return new Regex($"^{reg}$").IsMatch(value);
        }
        //static void Main(string[] args)
        //{
        //    // 시작 메시지 출력
        //    Console.WriteLine("AltASM 컴파일러 v0.0.1");
        //    Console.WriteLine($"지원하는 AltASM 최신 버전 : {max_version}");
        //    Console.Write("AltASM 코드 파일 경로 입력 > ");
        //    string in_file = Console.ReadLine();
        //    Console.Write("출력 파일 경로 입력 > ");
        //    string out_file = Console.ReadLine();
        //    Console.WriteLine("컴파일 중...");
        //    string[] in_str = File.ReadAllLines(in_file);
        //    int version = 1;
        //    bool used_version_syantx = false;
        //    string out_str = "";
        //    //if (in_str[0].StartsWith("#version"))
        //    //{
        //    //    version = int.Parse(in_str[0].Split(" ")[1]);
        //    //}
        //    //switch (version)
        //    //{
        //    //    case 1: // 버전 1
        //    //        foreach (var item in collection)
        //    //        {

        //    //        }
        //    //        break;
        //    //    default:
        //    //        PrintError(0, in_str[0], version.ToString());
        //    //        break;
        //    //}
        //    //Console.ResetColor();
        //    if (in_str[0].StartsWith("#version"))
        //    {
        //        version = int.Parse(in_str[0].Split(" ")[1]);
        //        used_version_syantx = true;
        //        if (version > max_version)
        //        {
        //            PrintError(0, in_str[0], version.ToString());
        //        }
        //        if (version < 1)
        //        {
        //            PrintError(1, in_str[0]);
        //        }
        //    }
        //    if (version == 1) // 버전 1
        //    {
        //        if (used_version_syantx)
        //        {
        //            out_str = comp_v1(in_str.Skip(1).ToArray());
        //        }
        //        else
        //        {
        //            out_str = comp_v1(in_str);
        //        }
        //    }
        //    if (out_file == "")
        //    {
        //        Console.WriteLine(out_str);
        //        return;
        //    }
        //    File.WriteAllText(out_file, out_str);
        //}
        static void Main(string[] args)
        {
       
            Console.WriteLine(new Regex("^(12)(.)*$").Match("12345").Groups[2].Captures[1]);
        }
    }
}