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
    public class MultiThreadController : ControllerBase
    {
        public static BlockingCollection<int> itensProcessar = new BlockingCollection<int>();
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
            itensProcessar.CompleteAdding();
        }

        [HttpGet]
        public void Adicionar()
        {
            Console.WriteLine("Adicionar");
            for (int i = 0; i < 20; i++)
                itensProcessar.Add(i);
        }

        [HttpGet]
        public void Processar()
        {
            Task.Run(async () =>
            {
                try
                {
                    SemaphoreSlim semaphore = new SemaphoreSlim(2);

                    while (true)
                    {
                        var item = itensProcessar.Take(src.Token);
                        Console.WriteLine("Item Para Processar :" + item);

                        await semaphore.WaitAsync();

                        Task.Run(async () =>
                        {
                            try
                            {
                                Console.WriteLine("Processando:" + item);
                                Thread.Sleep(3000);
                                Console.WriteLine("Terminado:" + item);
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception Multithread - {e.GetType().ToString()} - {e.Message}");
                }
            });
        }
    }
}
