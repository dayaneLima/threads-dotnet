using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Teste.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ThreadController : ControllerBase
    {
        public static BlockingCollection<int> bc = new BlockingCollection<int>();
        public static CancellationTokenSource src = new CancellationTokenSource();

        [HttpGet]
        public void Cancelar()
        {
            Console.WriteLine("Cancelar");
            src.Cancel(false);
        }

        [HttpGet]
        public void Finalizar()
        {
            Console.WriteLine("Finalizar");
            bc.CompleteAdding();
        }

        [HttpGet]
        public void Adicionar()
        {
            Console.WriteLine("Adicionar");
            for (int i = 0; i < 20; i++)
                bc.Add(i);
        }

        [HttpGet]
        public async void Processar()
        {
            Console.WriteLine("Processar 1");
            Task t2 = Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        Console.WriteLine("Processamento 1:" + bc.Take(src.Token));
                        Console.WriteLine("Processamento 1: While Depois");
                    }
                }
                catch (InvalidOperationException)
                {
                    // An InvalidOperationException means that Take() was called on a completed collection
                    Console.WriteLine("Processamento 1: That's All!");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Processamento 1: Take operation was canceled. IsCancellationRequested={0}", src.IsCancellationRequested);
                }
            });

            await Task.WhenAll(t2);
        }

        [HttpGet]
        public async void Processar2()
        {
            Console.WriteLine("Processar 2");
            Task t2 = Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        Console.WriteLine("Processamento 2: " + bc.Take(src.Token));
                        Console.WriteLine("Processamento 2: While Depois");
                    }
                }
                catch (InvalidOperationException)
                {
                    // An InvalidOperationException means that Take() was called on a completed collection
                    Console.WriteLine("Processamento 2: That's All!");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Processamento 2: Take operation was canceled. IsCancellationRequested={0}", src.IsCancellationRequested);
                }
            });

            await Task.WhenAll(t2);
        }
    }
}
