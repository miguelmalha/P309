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
    public class ContactosController : Controller
    {
        private DB db = new DB();

        // GET: Contactos
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            // ViewBag para os sorts
            //A ViewBag serve para transportar informações entre o controller e a view
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewBag.TelefoneSortParm = sortOrder == "Telefone" ? "Telefone_desc" : "Telefone";
            ViewBag.NotasSortParm = sortOrder == "Notas" ? "Notas_desc" : "Notas";
            ViewBag.EmpresaSortParm = sortOrder == "Empresa" ? "Empresa_desc" : "Empresa";
            ViewBag.NIFEmpresaSortParm = sortOrder == "NIFEmpresa" ? "NIFEmpresa_desc" : "NIFEmpresa";
            ViewBag.EmailEmpresaSortParm = sortOrder == "EmailEmpresa" ? "EmailEmpresa_desc" : "EmailEmpresa";
            ViewBag.SiteEmpresaSortParm = sortOrder == "SiteEmpresa" ? "SiteEmpresa_desc" : "SiteEmpresa";
            ViewBag.IBANEmpresaSortParm = sortOrder == "IBANEmpresa" ? "IBANEmpresa_desc" : "IBANEmpresa";


            //IF com a finalidade de confirmar se existe algum dado pesquisado. Se não retorna à primeira página, se sim continua com o filtro desejado
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString; //Guardar filtro corrente com o digitado pelo utilizador

            var contactos = from s in db.Contactos //Query para ir buscar os contactos
                            select s;

            contactos = db.Contactos.Include(d => d.Empresa); // Incluir as empresa no contacto. Código para mostrar a empresa na lista de contactos que, até então, estava vazia.

            // Se existir algo digitado na pesquisa por nome, aplica o filtro e apenas guarda os contactos que contém a string digitada
            if (!String.IsNullOrEmpty(searchString))
            {
                contactos = contactos.Where(s => s.Nome.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString; //Guardar novamente o filtro corrente digitado pelo utilizador

            //switch para definir a ordenação desejada pelo utilizador
            switch (sortOrder)
            {
                case "Nome_desc":
                    contactos = contactos.OrderByDescending(s => s.Nome);
                    break;
                case "Email":
                    contactos = contactos.OrderBy(s => s.Email);
                    break;
                case "Email_desc":
                    contactos = contactos.OrderByDescending(s => s.Email);
                    break;
                case "Telefone":
                    contactos = contactos.OrderBy(s => s.Telefone);
                    break;
                case "Telefone_desc":
                    contactos = contactos.OrderByDescending(s => s.Telefone);
                    break;
                case "Notas":
                    contactos = contactos.OrderBy(s => s.Notas);
                    break;
                case "Notas_desc":
                    contactos = contactos.OrderByDescending(s => s.Notas);
                    break;
                case "Empresa":
                    contactos = contactos.OrderBy(s => s.Empresa.Nome);
                    break;
                case "Empresa_desc":
                    contactos = contactos.OrderByDescending(s => s.Empresa.Nome);
                    break;
                case "NIFEmpresa":
                    contactos = contactos.OrderBy(s => s.Empresa.NIF);
                    break;
                case "NIFEmpresa_desc":
                    contactos = contactos.OrderByDescending(s => s.Empresa.NIF);
                    break;
                case "EmailEmpresa":
                    contactos = contactos.OrderBy(s => s.Empresa.Email);
                    break;
                case "EmailEmpresa_desc":
                    contactos = contactos.OrderByDescending(s => s.Empresa.Email);
                    break;
                case "SiteEmpresa":
                    contactos = contactos.OrderBy(s => s.Empresa.Site);
                    break;
                case "SiteEmpresa_desc":
                    contactos = contactos.OrderByDescending(s => s.Empresa.Site);
                    break;
                case "IBANEmpresa":
                    contactos = contactos.OrderBy(s => s.Empresa.IBAN);
                    break;
                case "IBANEmpresa_desc":
                    contactos = contactos.OrderByDescending(s => s.Empresa.IBAN);
                    break;
                default: // A primeira vez que a página é acedida o filtro pré definido é a ordenação ascendente por Nome do contacto
                    contactos = contactos.OrderBy(s => s.Nome);
                    break;
            }

            int pageSize = 3; //foi definido 3 artigos a mostrar por página
            int pageNumber = (page ?? 1); //retorna o valor da página ou 1 caso esta seja null
            return View(contactos.ToPagedList(pageNumber, pageSize)); //O método ToPagedList leva o numero da página e o numero de elementos presentes. retorno para a view dos contacto
        }

        // GET: Contactos/Detalhes
        public ActionResult Detalhes(int? id)
        {
            if (id == null) //Retorna código http para bad request se id do contacto for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contactos contactos = db.Contactos.Find(id); //Vai buscar o contacto que contém o id passado
            contactos.Empresa = db.Empresas.Where(a => a.NIF == contactos.EmpresaNIF).First(); // Vai procurar as empresas onde o NIF corresponde ao EmpresaNIF presente no contacto. Colocado porque não aparecia informação da empresa na página de detalhes.

            if (contactos == null) //se o contacto não existir, retorna código de erro para Not Found
            {
                return HttpNotFound();
            }

            return View(contactos); // retorno para a view 
        }

        // GET: Contactos/Criar
        public ActionResult Criar() 
        {
            ViewBag.EmpresaNIF = new SelectList(db.Empresas, "NIF", "Nome"); //Mostrar lista de empresas a selecionar para associar ao contacto na sua criação
            return View();
        }

        // POST: Contactos/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Criar([Bind(Include = "ID,Nome,Email,Telefone,Notas,EmpresaNIF")] Contactos contactos)
        {
            if (ModelState.IsValid) //Se o modelo for válido
            {
                db.Contactos.Add(contactos); //adiciona o novo contacto à bd
                db.SaveChanges(); //grava as altrações na bd
                return RedirectToAction("Index"); //redireciona para o index dos contactos
            }
            //Caso não seja válido
            ViewBag.EmpresaNIF = new SelectList(db.Empresas, "NIF", "Nome", contactos.EmpresaNIF); //nova listagem de empresas para associar ao contacto a criar
            return View(contactos); //retorno para a view de criação
        }

        // GET: Contactos/Editar
        public ActionResult Editar(int? id)
        {
            if (id == null)//Retorna código http para bad request se id do contacto for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contactos contactos = db.Contactos.Find(id);  //Vai buscar o contacto que contém o id passado

            ViewBag.EditarContactoPermanente = null; //ViewBag para passar para a view a informação de que se está atentar editar o contacto que não pode ser modificado
                                                    //Este contacto não pode ser modificado pois o seu id é utilizado como valor DEFAULT na BD, ou seja, se for apagado um contacto,
                                                    // os eventos que tinham esse contacto associado, passarão a ter como "Responsavel" o contacto com id =1

            if (id == 1) //Se o contacto a ser editado é o contacto com id =1
            {
                ViewBag.EditarContactoPermanente = "Este contacto não pode ser editado"; //mensagem de erro para informar que não se pode editar
                return View(); //retorno para a view
            }

            if (contactos == null) //Se o contacto não existir, código de http para Not Found
            {
                return HttpNotFound();
            }

            ViewBag.EmpresaNIF = new SelectList(db.Empresas, "NIF", "Nome", contactos.EmpresaNIF); //Nova listagem para selecão da empresa
            return View(contactos);//retorno para a view 
        }

        // POST: Contactos/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "ID,Nome,Email,Telefone,Notas,EmpresaNIF")] Contactos contactos)
        {
            if (ModelState.IsValid)//Se o modelo for válido
            {
                db.Entry(contactos).State = EntityState.Modified; //Altera estado para modificado
                db.SaveChanges(); //grava alterações na bd
                return RedirectToAction("Index"); //retorna para o index dos contactos
            }

            ViewBag.EmpresaNIF = new SelectList(db.Empresas, "NIF", "Nome", contactos.EmpresaNIF); //Em caso de erro, nova listagem das emrpesas
            return View(contactos); //retorno para a view
        }

        // GET: Contactos/Eliminar
        public ActionResult Eliminar(int? id)
        {
            if (id == null) //validar se id é nulo e retorna código http para Bad Request
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contactos contactos = db.Contactos.Find(id);  //Vai buscar o contacto que contém o id passado
            contactos.Empresa = db.Empresas.Where(a => a.NIF == contactos.EmpresaNIF).First(); // Vai procurar as empresas onde o NIF corresponde ao EmpresaNIF presente no contacto.

            ViewBag.ContactoPermanente = null; //ViewBag para guardar mensagem caso se esteja a eliminar o contacto que não pode ser eliminado

            if (id == 1) //Se o contacto a ser eliminado é o contacto com id =1
            {
                ViewBag.ContactoPermanente = "Este contacto não pode ser eliminado"; //mensagem para avisar que não se pode eliminar este contacto
                return View(); //Retorno para a view
            }

            if (contactos == null) //Caso contacto não exista, código http para Not Found
            {
                return HttpNotFound();
            }
            return View(contactos); //retorno para a view
        }

        // POST: Contactos/Eliminar
        [HttpPost, ActionName("Eliminar")] 
        [ValidateAntiForgeryToken]
        public ActionResult EliminacaoConfirmada(int id)
        {
            Contactos contactos = db.Contactos.Find(id); //Vai buscar o contacto que contém o id passado
            db.Contactos.Remove(contactos); //Remove o contacto da bd
            db.SaveChanges(); //grava as alterações na bd
            return RedirectToAction("Index"); // redireciona para o index dos contactos
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
