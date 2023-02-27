using System.Formats.Asn1;
using System.Xml.Linq;
using VirtualMemory2;
bool end = true;
VM vM = new VM();
vM.CreateObject();
while (end)
{
    int NumberVar;
    Console.WriteLine("Введите номер команды\n1.Заполнить значение по индеку\n2.Проверить наличие значения по индексу\n0.Выход");
    try { NumberVar = Convert.ToInt32(Console.ReadLine()); } catch(FormatException e) { Console.WriteLine(e); NumberVar = -1;} catch(OverflowException e) { Console.WriteLine(e); NumberVar = -1;};
    switch (NumberVar) 
    {
        case 1: {int value = 0; Console.WriteLine("Введите индекс");try { value = Convert.ToInt32(Console.ReadLine()); } catch (OverflowException e) { Console.WriteLine(e); break; } catch(FormatException e) { Console.WriteLine(e);break;};try { vM.WriteOnIndex(value); } catch (ArgumentOutOfRangeException e) { Console.WriteLine(e); break;} catch (OverflowException e) { Console.WriteLine(e); break; } break; }
        case 2: { int value = 0; Console.WriteLine("Введите индекс"); try { value = Convert.ToInt32(Console.ReadLine()); } catch (OverflowException e) { Console.WriteLine(e); break; } catch (FormatException e) { Console.WriteLine(e); break; }; try { vM.ByteMapInfo(value); } catch (ArgumentOutOfRangeException e) { Console.WriteLine(e); break; } catch (OverflowException e) { Console.WriteLine(e); break; } break; }
        case 0: {vM.SaveAll(); end = false; break;}
        default: { Console.WriteLine("Неверный номер операции , попробуйте снова"); break;}
    }  
}