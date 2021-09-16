using System;
using System.Collections.Generic;
using System.Data;
using PagedList;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using P309.DAL;
using P309.Models;

namespace P309.Controllers
{
    public class EventosController : Controller
    {
        private DB db = new DB();

        // GET: Eventos
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            // ViewBag para os sorts
            //A ViewBag serve para transportar informações entre o controller e a view
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DataInicioSortParm = String.IsNullOrEmpty(sortOrder) ? "DataInicio_desc" : "";
            ViewBag.NomeSortParm = sortOrder == "Nome" ? "Nome_desc" : "Nome";
            ViewBag.DataFimSortParm = sortOrder == "DataFim" ? "DataFim_desc" : "DataFim";
            ViewBag.LinkSortParm = sortOrder == "Link" ? "Link_desc" : "Link";
            ViewBag.LocalizacaoSortParm = sortOrder == "Localizacao" ? "Localizacao_desc" : "Localizacao";
            ViewBag.NotasSortParm = sortOrder == "Notas" ? "Notas_desc" : "Notas";

            //IF com a finalidade de confirmar se existe algum dado pesquisado. Se não retorna à primeira página, se sim continua com o filtro desejado
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString; //guarda valor do filtro atual

            var eventos = from s in db.Eventos //Query para ir buscar empresas
                            select s;

            // só é executada se existir um valor adicionado na caixa de pesquisa.
            if (!String.IsNullOrEmpty(searchString))
            {
                eventos = eventos.Where(s => s.Nome.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString;//guarda valor do filtro atual

            //switch para ver qual a ordenação desejada pelo utilizador
            switch (sortOrder)
            {
                case "Nome":
                    eventos = eventos.OrderBy(s => s.Nome);
                    break;
                case "Nome_desc":
                    eventos = eventos.OrderByDescending(s => s.Nome);
                    break;
                case "DataInicio_desc":
                    eventos = eventos.OrderByDescending(s => s.DataInicio);
                    break;
                case "DataFim":
                    eventos = eventos.OrderBy(s => s.DataFim);
                    break;
                case "DataFim_desc":
                    eventos = eventos.OrderByDescending(s => s.DataFim);
                    break;
                case "Link":
                    eventos = eventos.OrderBy(s => s.Link);
                    break;
                case "Link_desc":
                    eventos = eventos.OrderByDescending(s => s.Link);
                    break;
                case "Localizacao":
                    eventos = eventos.OrderBy(s => s.Localizacao);
                    break;
                case "Localizacao_desc":
                    eventos = eventos.OrderByDescending(s => s.Localizacao);
                    break;
                case "Notas":
                    eventos = eventos.OrderBy(s => s.Notas);
                    break;
                case "Notas_desc":
                    eventos = eventos.OrderByDescending(s => s.Notas);
                    break;
                default:  // A primeira vez que a página é acedida o filtro pré definido é a ordenação ascendente por Data de Inicio do Evento
                    eventos = eventos.OrderBy(s => s.DataInicio);
                    break;
            }

            int pageSize = 3;  //foi definido 3 artigos a mostrar por página
            int pageNumber = (page ?? 1); //retorna o valor da página ou 1 caso esta seja null
            return View(eventos.ToPagedList(pageNumber, pageSize)); //O método ToPagedList leva o numero da página 
        }

        // GET: Eventos/Detalhes
        public ActionResult Detalhes(int? id)
        {
            if (id == null) //Retorna código http para bad request se id dos eventos for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }

            Eventos eventos = db.Eventos.Find(id);  //Vai buscar o evento que contém o id passado

            if (eventos == null) //se o evento não existir, retorna código de erro para Not Found
            {
                return HttpNotFound();
            }

            return View(eventos); //retorno à view 
        }

        // GET: Eventos/Criar
        public ActionResult Criar()
        {
            return View();
        }

        // POST: Eventos/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Criar([Bind(Include = "ID,Nome,DataInicio,DataFim,Link,Localizacao,Notas")] Eventos eventos)
        {
            if (ModelState.IsValid) //se modelo válido
            {
                db.Eventos.Add(eventos); //adiciona novo evento
                db.SaveChanges(); //guarda alterações na bd
                return RedirectToAction("Index"); //retorna para o index dos eventos
            }

            return View(eventos); //retorna para a view
        }

        // GET: Eventos/Editar
        public ActionResult Editar(int? id)
        {
            if (id == null) //Retorna código http para bad request se id dos eventos for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }

            Eventos eventos = db.Eventos.Find(id); //Vai buscar o evento que contém o id passado

            if (eventos == null) //se o evento não existir, retorna código de erro para Not Found
            {
                return HttpNotFound();
            }

            return View(eventos); //retorno para a view
        }

        // POST: Eventos/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "ID,Nome,DataInicio,DataFim,Link,Localizacao,Notas")] Eventos eventos)
        {
            if (ModelState.IsValid) //se modelo válido
            {
                db.Entry(eventos).State = EntityState.Modified; //modifica o evento
                db.SaveChanges(); //grava alterações
                return RedirectToAction("Index"); //retorna ao index de eventos
            }

            return View(eventos); //retorna à view
        }

        // GET: Eventos/Eliminar
        public ActionResult Eliminar(int? id)
        {
            if (id == null) //Retorna código http para bad request se id do evento for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }

            Eventos eventos = db.Eventos.Find(id);//Vai buscar o evento que contém o id passado

            if (eventos == null) //se o evento não existir, retorna código de erro para Not Found
            {
                return HttpNotFound();
            }

            return View(eventos); //retorno à view
        }

        // POST: Eventos/Eliminar
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminacaoConcluida(int id)
        {
            Eventos eventos = db.Eventos.Find(id); //buscar evento com id passado
            db.Eventos.Remove(eventos); //remove evento
            db.SaveChanges(); //grava alterações na bd
            return RedirectToAction("Index"); //retorno ao index dos eventos
        }

        protected override void Dispose(bool disposing) //fechar ligação à bd
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
