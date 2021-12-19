using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using WebCRUDMVCSQL.Controllers;
using WebCRUDMVCSQL.Models;
using System.Data;

namespace WebCRUDMVCSQL.Testes
{
    [TestFixture]
    public class DatabaseTest
    {
        private Contexto _context;

        [SetUp]
        public void Setup()
        {
            var connectionString = "Data Source=DESKTOP-3PKSHGP;Initial Catalog=Agoravaiiii;Persist Security Info=True;User ID=tesco;Password=g0d0fw@r; TrustServerCertificate=True;";
            var options = new DbContextOptionsBuilder<Contexto>()
                .UseSqlServer(connectionString)
                .Options;
            _context = new Contexto(options);
        }

        [Test]
        public void OpenAndCloseConnection()
        {
            var connection = _context.Database.GetDbConnection();

            connection.Open();
            Assert.AreEqual(ConnectionState.Open, connection.State);
            connection.Close();
            Assert.AreEqual(ConnectionState.Closed, connection.State);
        }

        [Test]
        public async Task CreateProduto()
        {
            var controller = new ProdutosController(_context);
            var newProduct = new Produto { Id = 100, Nome = "ProdutoTeste" };

            var result = await controller.Create(newProduct);

            var createdProduct = await _context.Produto.FirstOrDefaultAsync(p => p.Nome == "ProdutoTeste");
            Assert.IsNotNull(createdProduct);
            Assert.AreEqual(100, createdProduct.Id);
            Assert.AreEqual("ProdutoTeste", createdProduct.Nome);
        }

        [Test]
        public async Task DeleteProduto()
        {

            var existingProduct = new Produto { Id = 100, Nome = "ProdutoTeste" };
            await _context.Produto.AddAsync(existingProduct);
            await _context.SaveChangesAsync();

            var controller = new ProdutosController(_context);

            var result = await controller.DeleteConfirmed(existingProduct.Id);

            var deletedProduct = await _context.Produto.FirstOrDefaultAsync(p => p.Id == existingProduct.Id);
            Assert.IsNull(deletedProduct);
        }

        [Test]
        public async Task UpdateProduto()
        {
            var existingProduct = new Produto { Id = 100, Nome = "ProdutoTeste" };
            await _context.Produto.AddAsync(existingProduct);
            await _context.SaveChangesAsync();

            var controller = new ProdutosController(_context);

            var updatedProduct = new Produto { Id = existingProduct.Id, Nome = "UpdatedProdutoTeste" };
            var result = await controller.Edit(existingProduct.Id, updatedProduct);

            var productAfterUpdate = await _context.Produto.FindAsync(existingProduct.Id);
            Assert.IsNotNull(productAfterUpdate);
            Assert.AreEqual("UpdatedProdutoTeste", productAfterUpdate.Nome);
        }
    }
}
