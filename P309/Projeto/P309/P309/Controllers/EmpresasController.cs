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
    public class EmpresasController : Controller
    {
        private DB db = new DB();

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            // ViewBag para os sorts
            //A ViewBag serve para transportar informações entre o controller e a view
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NomeSortParm = String.IsNullOrEmpty(sortOrder) ? "Nome_desc" : "";
            ViewBag.NIFSortParm = sortOrder == "NIF" ? "NIF_desc" : "NIF";
            ViewBag.EnderecoSortParm = sortOrder == "Endereco" ? "Endereco_desc" : "Endereco";
            ViewBag.PaisSortParm = sortOrder == "Pais" ? "Pais_desc" : "Pais";
            ViewBag.LocalidadeSortParm = sortOrder == "Localidade" ? "Localidade_desc" : "Localidade";
            ViewBag.CodigoPostalSortParm = sortOrder == "CodigoPostal" ? "CodigoPostal_desc" : "CodigoPostal";
            ViewBag.SiteSortParm = sortOrder == "Site" ? "Site_desc" : "Site";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewBag.TelefoneSortParm = sortOrder == "Telefone" ? "Telefone_desc" : "Telefone";
            ViewBag.IBANSortParm = sortOrder == "IBAN" ? "IBAN_desc" : "IBAN";
            ViewBag.MeioDePagamentoSortParm = sortOrder == "MeioDePagamento" ? "MeioDePagamento_desc" : "MeioDePagamento";
            ViewBag.PrazoDePagamentoSortParm = sortOrder == "PrazoDePagamento" ? "PrazoDePagamento_desc" : "PrazoDePagamento";
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

            ViewBag.CurrentFilter = searchString; //guarda o valor do filtro atual

            var empresas = from s in db.Empresas //Query para ir buscar as empresas
                           select s;

            // Se existir algo digitado na pesquisa por nome, apenas guarda as empresas que contém a string digitada
            if (!String.IsNullOrEmpty(searchString))
            {
                empresas = empresas.Where(s => s.Nome.Contains(searchString)); //aplica o filtro
            }

            ViewBag.CurrentFilter = searchString; //Guardar novamente o filtro corrente digitado pelo utilizador

            //switch para ver qual a ordenação desejada pelo utilizador
            switch (sortOrder)
            {
                case "Nome_desc":
                    empresas = empresas.OrderByDescending(s => s.Nome);
                    break;
                case "NIF":
                    empresas = empresas.OrderBy(s => s.NIF);
                    break;
                case "NIF_desc":
                    empresas = empresas.OrderByDescending(s => s.NIF);
                    break;
                case "Endereco":
                    empresas = empresas.OrderBy(s => s.Endereço);
                    break;
                case "Endereco_desc":
                    empresas = empresas.OrderByDescending(s => s.Endereço);
                    break;
                case "Pais":
                    empresas = empresas.OrderBy(s => s.País);
                    break;
                case "Pais_desc":
                    empresas = empresas.OrderByDescending(s => s.País);
                    break;
                case "Localidade":
                    empresas = empresas.OrderBy(s => s.Localidade);
                    break;
                case "Localidade_desc":
                    empresas = empresas.OrderByDescending(s => s.Localidade);
                    break;
                case "CodigoPostal":
                    empresas = empresas.OrderBy(s => s.CódigoPostal);
                    break;
                case "CodigoPostal_desc":
                    empresas = empresas.OrderByDescending(s => s.CódigoPostal);
                    break;
                case "Site":
                    empresas = empresas.OrderBy(s => s.Site);
                    break;
                case "Site_desc":
                    empresas = empresas.OrderByDescending(s => s.Site);
                    break;
                case "Email":
                    empresas = empresas.OrderBy(s => s.Email);
                    break;
                case "Email_desc":
                    empresas = empresas.OrderByDescending(s => s.Email);
                    break;
                case "Telefone":
                    empresas = empresas.OrderBy(s => s.Telefone);
                    break;
                case "Telefone_desc":
                    empresas = empresas.OrderByDescending(s => s.Telefone);
                    break;
                case "IBAN":
                    empresas = empresas.OrderBy(s => s.IBAN);
                    break;
                case "IBAN_desc":
                    empresas = empresas.OrderByDescending(s => s.IBAN);
                    break;
                case "MeioDePagamento":
                    empresas = empresas.OrderBy(s => s.MeioDePagamento);
                    break;
                case "MeioDePagamento_desc":
                    empresas = empresas.OrderByDescending(s => s.MeioDePagamento);
                    break;
                case "PrazoDePagamento":
                    empresas = empresas.OrderBy(s => s.PrazoDePagamento);
                    break;
                case "PrazoDePagamento_desc":
                    empresas = empresas.OrderByDescending(s => s.PrazoDePagamento);
                    break;
                case "Notas":
                    empresas = empresas.OrderBy(s => s.Notas);
                    break;
                case "Notas_desc":
                    empresas = empresas.OrderByDescending(s => s.Notas);
                    break;
                default:  // A primeira vez que a página é acedida o filtro pré definido é a ordenação ascendente por Nome 
                    empresas = empresas.OrderBy(s => s.Nome);
                    break;
            }

            int pageSize = 3; //foi definido 3 artigos a mostrar por página
            int pageNumber = (page ?? 1); //retorna o valor da página ou 1 caso esta seja null
            return View(empresas.ToPagedList(pageNumber, pageSize)); //O método ToPagedList leva o numero da página
        }

        // GET: Empresas/Detalhes
        public ActionResult Detalhes(string id)
        {
            if (id == null) //Retorna código http para bad request se id da empresa for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); 
            }

            Empresas empresas = db.Empresas.Find(id); //Vai buscar a empresa que contém o id passado
            empresas.Contactos = db.Contactos.Where(a => a.EmpresaNIF == empresas.NIF).ToList(); // Vai procurar as empresas onde o NIF corresponde ao EmpresaNIF presente no contacto. Colocado porque não aparecia informação dos contactos associados na página de detalhes.

            if (empresas == null) //se a empresa não existir, retorna código de erro para Not Found
            {
                return HttpNotFound();
            }

            return View(empresas); //retorna para a view
        }

        // GET: Empresas/Criar
        public ActionResult Criar()
        {
            return View();
        }

        // POST: Empresas/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Criar([Bind(Include = "NIF,Nome,Endereço,País,Localidade,CódigoPostal,Site,Email,Telefone,IBAN,MeioDePagamento,PrazoDePagamento,Notas")] Empresas empresas)
        {
            //ViewBags para o caso de o utilizador não digitar o nif ou digitar um nif já existente
            ViewBag.NIFExistente = null;
            ViewBag.NIFInexistente = null;
            
            if (ModelState.IsValid) //Se o modelo for válido
            {
                try
                {
                    if (empresas.NIF == null) //se o NIF não for digitado
                    {
                        ViewBag.NIFInexistente = "Insira um NIF"; //mensagem a avisar que é necessário inserir um NIF
                        return View(empresas); //retorno para a view 
                    }

                    var NIFExistente = db.Empresas.Where(a => a.NIF == empresas.NIF).First(); //Procurar se já existe um nif com o valor digitado. Caso não haja, o programa salta para a exceção e cria a empresa 
                    ViewBag.NIFExistente = "O NIF inserido já está associado a uma empresa"; // se for encontrado nif já existente, avisa o utilizador do mesmo
                    return View(empresas); //retorna para a view
                }
                catch (Exception) //acontece quando o nif inserido é válido e dá inicio à inserção da empresa
                {
                    db.Empresas.Add(empresas); // adiciona a empresa na bd
                    db.SaveChanges(); //grava as alterações
                    return RedirectToAction("Index"); //retorna ao index
                }
            }

            return View(empresas); //retorno à view
        }

        // GET: Empresas/Editar
        public ActionResult Editar(string id)
        {
            if (id == null) //Retorna código http para bad request se id(nif) da empresa for null
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            ViewBag.EditarEmpresaPermanente = null; //ViewBag para passar para a view a informação de que se está atentar editar a empresa que não pode ser editada
                                                    //Esta empresa não pode ser modificada pois o seu id é utilizado como valor DEFAULT na BD, ou seja, se for apagada uma empresa,
                                                    // os contacto associados a essa empresa passarão a ter como "Empresa" a empresa com id ="123456789"

            ViewBag.EditarNIF = "Não é possível editar o NIF da empresa. Para o fazer terá de criar uma nova empresa."; //Sendo o NIF uma PK, não é boa politica alterar o valor da mesma. Por este motivo, impede-se a sua alteração e reporta-se a mensagem ao utilizador

            Empresas empresas = db.Empresas.Find(id); //Vai buscar a empresa que contém o id passado

            if (id == "123456789") //Se a empresa a ser editada é a com id ="123456789"
            {
                ViewBag.EditarEmpresaPermanente = "Esta empresa não pode ser editada"; //aviso que a empresa não pode ser editada
                return View(empresas); //retorno para a view
            }

            if (empresas == null) //Se a empresa não existir, código de http para Not Found
            {
                return HttpNotFound();
            }

            return View(empresas); //retorno para a view
        }

        // POST: Empresas/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "NIF,Nome,Endereço,País,Localidade,CódigoPostal,Site,Email,Telefone,IBAN,MeioDePagamento,PrazoDePagamento,Notas")] Empresas empresas)
        {
            if (ModelState.IsValid) //Se o modelo for válido
            {
                db.Entry(empresas).State = EntityState.Modified; //Altera estado para modificado
                db.SaveChanges(); //grava alterações na bd
                return RedirectToAction("Index"); //retorna para index das empresas
            }

            return View(empresas); //retorna para a view
        }

        // GET: Empresas/Eliminar
        public ActionResult Eliminar(int? id, bool? saveChangesError = false)
        {
            if (id == null)//validar se id é nulo e retorna código http para Bad Request
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault()) //Se existir um erro avisa o utilizador
            {
                ViewBag.ErrorMessage = "Eliminação falhada. Tente novamente";
            }

            Empresas empresas = db.Empresas.Find(id.ToString()); //Vai buscar a empresa que contém o id passado. ToString porque o id é um string

            ViewBag.EmpresaPermanente = null; //ViewBag para guardar mensagem caso se esteja a eliminar a empresa que não pode ser eliminada

            if (id == 123456789) //Se a empresa a ser eliminado é a com id = 123456789
            {
                ViewBag.EmpresaPermanente = "Esta empresa não pode ser eliminada"; //avisa o utilizador
                return View(); //retorna para a view
            }

            if (empresas == null)  //Caso empresa não exista, código http para Not Found
            {
                return HttpNotFound();
            }

            return View(empresas); //retorno à view
        }

        // POST: Empresas/Eliminar
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminacaoConluida(int id)
        {
            try
            {
                Empresas Eliminar = new Empresas() { NIF =  id.ToString() }; //criada uma empresa apenas com o mesmo id da empresa a eliminar
                db.Entry(Eliminar).State = EntityState.Deleted; //empresa marcafa como apagada
                db.Empresas.Remove(Eliminar); //eliminada a empresa criada. Esta forma de apagar a empresa é suficiente para que o entity framework consiga apagar a entidade empresa
                db.SaveChanges(); //gravar alterações
            }
            catch (DataException)
            {
                return RedirectToAction("Eliminar", new { id = id.ToString(), saveChangesError = true }); //caso exista um erro, o usuário é avisado
            }
            return RedirectToAction("Index"); //retorno para o index das empresas
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
