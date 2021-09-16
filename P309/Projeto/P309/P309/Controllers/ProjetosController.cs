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
    public class ProjetosController : Controller
    {
        private DB db = new DB();

        // GET: Projetos
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            // ViewBag para os sorts
            //A ViewBag serve para transportar informações entre o controller e a view
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";
            ViewBag.NumeroProjetoSortParm = sortOrder == "NumeroProjeto" ? "NumeroProjeto_desc" : "NumeroProjeto";
            ViewBag.AreaSortParm = sortOrder == "Area" ? "Area_desc" : "Area";
            ViewBag.EstadoSortParm = sortOrder == "Estado" ? "Estado_desc" : "Estado";
            ViewBag.DataDeCriacaoSortParm = sortOrder == "DataDeCriacao" ? "DataDeCriacao_desc" : "DataDeCriacao";
            ViewBag.NotasSortParm = sortOrder == "Notas" ? "Notas_desc" : "Notas";
            ViewBag.ResponsavelSortParm = sortOrder == "Responsavel" ? "Responsavel_desc" : "Responsavel";

            //IF com a finalidade de confirmar se existe algum dado pesquisado. Se não retorna à primeira página, se sim continua com o filtro desejado
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString; //grava o filtro atual

            var projetos = from s in db.Projetos //Query para ir buscar os projetos
                           select s;

            projetos = db.Projetos.Include(c => c.Contacto); //Incluir o contacto. Sem esta linha não havia referência do contacto ao projeto

            // só é executada se existir um valor adicionado na caixa de pesquisa.
            if (!String.IsNullOrEmpty(searchString))
            {
                projetos = projetos.Where(s => s.Nome.Contains(searchString)); //aplica o filtro
            }

            ViewBag.CurrentFilter = searchString; //grava o filtro atual

            //switch para ver qual a ordenação desejada pelo utilizador
            switch (sortOrder)
            {
                case "Nome_desc":
                    projetos = projetos.OrderByDescending(s => s.Nome);
                    break;
                case "NumeroProjeto":
                    projetos = projetos.OrderBy(s => s.NumeroProjeto);
                    break;
                case "NumeroProjeto_desc":
                    projetos = projetos.OrderByDescending(s => s.NumeroProjeto);
                    break;
                case "Area":
                    projetos = projetos.OrderBy(s => s.Area);
                    break;
                case "Area_desc":
                    projetos = projetos.OrderByDescending(s => s.Area);
                    break;
                case "Estado":
                    projetos = projetos.OrderBy(s => s.Estado);
                    break;
                case "Estado_desc":
                    projetos = projetos.OrderByDescending(s => s.Estado);
                    break;
                case "DataDeCriacao":
                    projetos = projetos.OrderBy(s => s.DataDeCriaçao);
                    break;
                case "DataDeCriacao_desc":
                    projetos = projetos.OrderByDescending(s => s.DataDeCriaçao);
                    break;
                case "Notas":
                    projetos = projetos.OrderBy(s => s.Notas);
                    break;
                case "Notas_desc":
                    projetos = projetos.OrderByDescending(s => s.Notas);
                    break;
                case "Responsavel":
                    projetos = projetos.OrderBy(s => s.Contacto.Nome);
                    break;
                case "Responsavel_desc":
                    projetos = projetos.OrderByDescending(s => s.Contacto.Nome);
                    break;
                default: // A primeira vez que a página é acedida o filtro pré definido é a ordenação ascendente por Nome 
                    projetos = projetos.OrderBy(s => s.Nome);
                    break;
            }

            int pageSize = 3;  //foi definido 3 artigos a mostrar por página
            int pageNumber = (page ?? 1); //retorna o valor da página ou 1 caso esta seja null
            return View(projetos.ToPagedList(pageNumber, pageSize)); //O método ToPagedList leva o numero da página 
        }

        // GET: Projetos/Detalhes
        public ActionResult Detalhes(int? id)
        {
            if (id == null) //Retorna código http para bad request se id do projeto for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }

            Projetos projetos = db.Projetos.Find(id); //Vai buscar o projeto que contém o id passado
            projetos.Contacto = db.Contactos.Where(a => a.ID == projetos.ContactoID).First(); // Vai procurar os contacto onde o ID corresponde ao ContactoID presente nos projetos. Colocado porque não aparecia informação dos contactos "Responsáveis" na página de detalhes.

            if (projetos == null) //se o projeto não existir, retorna código de erro para Not Found
            {
                return HttpNotFound();
            }
            return View(projetos); //retorna à view
        }

        // GET: Projetos/Criar
        public ActionResult Criar()
        {
            ViewBag.ContactoID = new SelectList(db.Contactos, "ID", "Nome"); //Listagem para os contactos a associar ao projeto
            return View();  //retorna à view
        }

        // POST: Projetos/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Criar([Bind(Include = "NumeroProjeto,Nome,Area,Estado,DataDeCriaçao,Notas,ContactoID")] Projetos projetos)
        {
            if (ModelState.IsValid) //se modelo válido
            {
                db.Projetos.Add(projetos); //adiciona o projeto
                db.SaveChanges(); //grava as alterações
                return RedirectToAction("Index"); //retorna ao index dos projetos
            }

            ViewBag.ContactoID = new SelectList(db.Contactos, "ID", "Nome", projetos.ContactoID); //listagem para os contacto a associar ao projeto
            return View(projetos); //retorna à view
        }

        // GET: Projetos/Editar
        public ActionResult Editar(int? id)
        {
            if (id == null) //Retorna código http para bad request se id do projeto for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //erro se projeto não encontrado
            }

            Projetos projetos = db.Projetos.Find(id); //Vai buscar o projeto que contém o id passado

            if (projetos == null)  //se o projeto não existir, retorna código de erro para Not Found
            {
                return HttpNotFound();
            }

            ViewBag.ContactoID = new SelectList(db.Contactos, "ID", "Nome", projetos.ContactoID);  //listagem para os contacto a associar ao projeto
            return View(projetos); //retorna à view
        }

        // POST: Projetos/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "NumeroProjeto,Nome,Area,Estado,DataDeCriaçao,Notas,ContactoID")] Projetos projetos)
        {
            if (ModelState.IsValid) //se modelo válido
            {
                db.Entry(projetos).State = EntityState.Modified; //altera estado para modificado
                db.SaveChanges(); //grava alterações na bd
                return RedirectToAction("Index"); // redireciona para o index dos projetos
            }

            ViewBag.ContactoID = new SelectList(db.Contactos, "ID", "Nome", projetos.ContactoID);  //listagem para os contacto a associar ao projeto
            return View(projetos); //retorna à view
        }

        // GET: Projetos/Eliminar
        public ActionResult Eliminar(int? id)
        {
            if (id == null) //Retorna código http para bad request se id do projeto for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Projetos projetos = db.Projetos.Find(id); //Vai buscar o projeto que contém o id passado
            projetos.Contacto = db.Contactos.Where(a => a.ID == projetos.ContactoID).First(); // Vai procurar os contacto onde o ID corresponde ao ContactoID presente nos projetos. 

            if (projetos == null) //se o projeto não existir, retorna código de erro para Not Found
            {
                return HttpNotFound();
            }

            return View(projetos); //retorna à view
        }

        // POST: Projetos/Eliminar
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminacaoConluida(int id)
        {
            Projetos projetos = db.Projetos.Find(id); //Vai buscar o projeto que contém o id passado
            db.Projetos.Remove(projetos); //remove o projeto
            db.SaveChanges(); //grava alterações
            return RedirectToAction("Index"); //redireciona para o index das empresas
        }

        protected override void Dispose(bool disposing) //fechar a ligação à bd
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
