using System;
using System.Collections.Generic;
using PagedList;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using P309.DAL;
using P309.Models;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace P309.Controllers
{
    public class ContasController : Controller
    {
        private DB db = new DB();

        // GET: Contas
        public ActionResult Index(Conta conta)
        {
            //ViewBags para retornar mensagens na View das Contas
            ViewBag.NotValidUser = null;
            ViewBag.NoUser = null;
            ViewBag.NoPass = null;
            ViewBag.NotValidPass = null;
            ViewBag.ShowPage = null;

            if (conta.NomeDeUtilizador != null && conta.PalavraPasse != null) //confirmar que foram inseridos dados no utilizador e na palavra-passe
            {
                try
                {
                    var cont = db.Conta.Where(a => a.NomeDeUtilizador == conta.NomeDeUtilizador).First(); //limitar a pesquisa ao utilizador e ver se este existe na bd
                    
                    if (conta.PalavraPasse == cont.PalavraPasse) //Ver se a palavra-passe digitada coincide com a do utilizador inserido
                    {
                        Session["Utilizador"] = cont.NomeDeUtilizador; // c# não tem variáveis globais. Forma de guardar o utilizador que está Logado
                        Session["RoleUtilizador"] = cont.Role; // Ir buscar a Role do utilizador que está Logado 
                        return Redirect("/Inicio/Index"); //caso o utiliador exista, redireciona para o sistema
                    }
                    else 
                    {
                    ViewBag.NotValidPass = "Palavra-Passe inválida"; //caso contrário informa da palavra passe inválida
                    }

                }
                catch(Exception)
                {
                    ViewBag.NotValidUser = "Login Inválido"; //Invalido se utilizador inexistente
                    return View(); 

                }

            }if (conta.NomeDeUtilizador == null)
            {
                ViewBag.NoUser = "Login Inválido"; //Se não existir utilizador informa do login Inválido

            }

            if (conta.PalavraPasse == null)
            {
                ViewBag.NoPass = "Login Inválido";  //Se não existir palavra passe informa do login Inválido

            }
            return View();
        }


        public ActionResult Acessos(string sortOrder, string currentFilter, string searchString, int? page)
          {
            //ViewBags para os sorts
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeDeUtilizadorSortParm = String.IsNullOrEmpty(sortOrder) ? "NomeDeUtilizador_desc" : "";
            ViewBag.PalavraPasseSortParm = sortOrder == "PalavraPasse" ? "PalavraPasse_desc" : "PalavraPasse";
            ViewBag.RoleSortParm = sortOrder == "Role" ? "Role_desc" : "Role";

            //IF com a finalidade de confirmar se existe algum dado pesquisado. Se não, retorna à primeira página, se sim continua com o filtro desejado
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString; //guarda o filtro atual

            var contas = from s in db.Conta //Query para ir buscar as contas
                            select s;

            // só é executada se existir uma string adicionada na caixa de pesquisa.
            if (!String.IsNullOrEmpty(searchString))
            {
                contas = contas.Where(s => s.NomeDeUtilizador.Contains(searchString)); //aplica o filtro 
            }

            ViewBag.CurrentFilter = searchString; //Guarda o filtro atual

            //switch para ver qual a ordenação desejada pelo utilizador
            switch (sortOrder)
            {
                case "NomeDeUtilizador_desc":
                    contas = contas.OrderByDescending(s => s.NomeDeUtilizador);
                    break;
                case "PalavraPasse":
                    contas = contas.OrderBy(s => s.PalavraPasse);
                    break;
                case "PalavraPasse_desc":
                    contas = contas.OrderByDescending(s => s.PalavraPasse);
                    break;
                case "Role":
                    contas = contas.OrderBy(s => s.Role);
                    break;
                case "Role_desc":
                    contas = contas.OrderByDescending(s => s.Role);
                    break;
                default: // A primeira vez que a página é acedida o filtro pré definido é a ordenação ascendente por Nome de utilizador
                    contas = contas.OrderBy(s => s.NomeDeUtilizador);
                    break;
            }

            int pageSize = 3;  //foi definido 3 artigos a mostrar por página
            int pageNumber = (page ?? 1); //retorna o valor da página ou 1 caso esta seja null
            return View(contas.ToPagedList(pageNumber, pageSize)); //O método ToPagedList leva o numero da página 
        }
           
        //Página de Perfil
        public ActionResult Perfil()
        {
            ViewBag.NotAdmin = null; //ViewBag para o caso do utilizador não ser um administrador
            string Utilizador = (string)Session["Utilizador"]; // Ir buscar o utilizador que está Logado

            int Role = (int)Session["RoleUtilizador"]; //Guardar a Role do utilizador que está logado

            if (Role != 0) //Se o utilizador não for administrador, avisa-o que tem de pedir ao administrador caso queria alterar as credenciais 
            {
                ViewBag.NotAdmin = "Caso queira alterar as suas credenciais, contacte o seu administrador";
            }

            Conta conta = db.Conta.Where(s => s.NomeDeUtilizador.Contains(Utilizador)).First(); //Ir buscar as informações da conta do utilizador que está logado

            return View(conta);
        }

        // GET: Contas/Detalhes
        public ActionResult Detalhes(int? id)
        {
            if (id == null) //Retorna código http para bad request se id da conta for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            } 

            Conta conta = db.Conta.Find(id); //conta com o id passado

            if (conta == null) //se a conta não existir, retorna código de erro para Not Found
            {
                return HttpNotFound();
            }

            return View(conta);
        }

        // GET: Contas/Criar
        public ActionResult Criar()
        {
            return View();
        }

        // POST: Contas/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Criar([Bind(Include = "ID,NomeDeUtilizador,PalavraPasse,Role")] Conta conta)
        {
            ViewBag.RoleError = null; //ViewBag para informar de erro na escolha da Role
     
            if (conta.Role != 0 && conta.Role != 1) //Se Role digitada for diferente de 0 ou 1 avisa o utilizador
            {
                ViewBag.RoleError = "Role deve ser 0 para Administrador e 1 para Visualizador";
                return View(); //retorna a view
            }

            if (ModelState.IsValid) //se Modelo válido
            {
                db.Conta.Add(conta); //adiciona nova conta
                db.SaveChanges(); //Grava alterações na db
                return RedirectToAction("Acessos"); //retorna à página de acessos
            }

            return View(conta);
        }

        // GET: Contas/Editar
        public ActionResult Editar(int? id)
        {
            if (id == null) //Retorna código http para bad request se id da conta for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }

            Conta conta = db.Conta.Find(id); //vai buscar a conta com o id passado

            if (conta == null) //Se conta não for encontrada, erro http Not Found
            {
                return HttpNotFound();
            }

            return View(conta); //retorna à view
        }

        // POST: Contas/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "ID,NomeDeUtilizador,PalavraPasse,Role")] Conta conta)
        {
            ViewBag.RoleError = null;//ViewBag para informar de erro na escolha da Role

            if (conta.Role != 0 && conta.Role != 1) //Se Role digitada for diferente de 0 ou 1 avisa o utilizador
            {
                ViewBag.RoleError = "Role deve ser 0 para Administrador e 1 para Visualizador";
                return View(); //retorno à view
            }

            if (ModelState.IsValid) //se Modelo válido
            {
                db.Entry(conta).State = EntityState.Modified; //Altera valores da conta
                db.SaveChanges(); //grava as alterações
                return RedirectToAction("Acessos");//retorna à pagina de acessos
            }

            return View(conta); //retorna a view
        }

        // GET: Contas/Editar a página de perfil
        public ActionResult EditarPerfil(int? id)
        {
            if (id == null) //Retorna código http para bad request se id da conta for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }

            Conta conta = db.Conta.Find(id); //Vai buscar a conta com o id passado

            if (conta == null) //Retorna código http para Not Found se conta não existir
            {
                return HttpNotFound();
            }

            return View(conta); //retorno à view
        }

        // POST: Contas/Editar a página de perfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPerfil([Bind(Include = "ID,NomeDeUtilizador,PalavraPasse,Role")] Conta conta)
        {
            ViewBag.RoleError = null; //ViewBag para informar de erro na escolha da Role

            if (conta.Role != 0 && conta.Role != 1) //Se role digitada for diferente de 0 ou 1 avisa o utilziador
            {
                ViewBag.RoleError = "Role deve ser 0 para Administrador e 1 para Visualizador";
                return View(); //retorna à view
            }

            if (ModelState.IsValid) //se modelo válido
            {
                db.Entry(conta).State = EntityState.Modified; //Altera valores da conta
                db.SaveChanges(); //grava as alterações
                return RedirectToAction("Perfil"); //retorna à página de perfil
            }

            return View(conta); //retorno à view
        }

        // GET: Contas/Eliminar
        public ActionResult Eliminar(int? id)
        {
            if (id == null) //Retorna código http para bad request se id da conta for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }

            Conta conta = db.Conta.Find(id); //buscar conta com o id passado

            if (conta == null) //Retorna código http para Not Found se conta não existir
            {
                return HttpNotFound();
            }

            return View(conta); //retorna à view
        }

        // POST: Contas/Eliminar
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminacaoConcluida(int id)
        {
            Conta conta = db.Conta.Find(id); //encontra a conta com o id passado
            db.Conta.Remove(conta); //Remove a conta
            db.SaveChanges(); //Grava alterações
            return RedirectToAction("Acessos"); //retorna aos acessos
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
