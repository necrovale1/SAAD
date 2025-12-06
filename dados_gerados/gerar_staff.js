const admin = require("firebase-admin");
const { fakerPT_BR: faker } = require("@faker-js/faker");
const serviceAccount = require("./serviceAccountKey.json");

// Inicializa o Firebase
if (!admin.apps.length) {
  admin.initializeApp({
    credential: admin.credential.cert(serviceAccount)
  });
}

const db = admin.firestore();
const auth = admin.auth();

const SENHA_PADRAO = "123456";

// Áreas para distribuir os professores
const AREAS = ["TI", "SAUDE", "GESTAO", "EXATAS", "AGRARIAS", "HUMANAS", "INDUSTRIA", "ARTE", "DIREITO"];

async function criarUsuario(nome, email, tipo, dadosExtras = {}) {
  try {
    // 1. Cria no Authentication
    let userRecord;
    try {
      userRecord = await auth.createUser({
        email: email,
        password: SENHA_PADRAO,
        displayName: nome,
      });
    } catch (e) {
      if (e.code === 'auth/email-already-exists') {
        console.log(`⚠️ E-mail já existe, pulando criação de Auth: ${email}`);
        const user = await auth.getUserByEmail(email);
        userRecord = user;
      } else {
        throw e;
      }
    }

    // 2. Salva no Firestore (Coleção 'users')
    await db.collection('users').doc(userRecord.uid).set({
      Nome: nome,
      Email: email,
      UserType: tipo, // "Gestor" ou "Professor"
      ...dadosExtras // Campos extras como 'Cargo' ou 'Departamento'
    }, { merge: true });

    console.log(`✅ [${tipo}] Criado: ${nome} (${email})`);

  } catch (error) {
    console.error(`❌ Erro ao criar ${nome}:`, error.message);
  }
}

async function rodarScript() {
  console.log("🚀 Iniciando criação da equipe (Staff)...");

  // --- 1. CRIAR GESTORES ---
  await criarUsuario("Admin TI", "admin.ti@escola.com", "Gestor", {
    Cargo: "Administrador de Sistemas",
    Departamento: "Tecnologia"
  });

  await criarUsuario("Diretor Geral", "diretor@escola.com", "Gestor", {
    Cargo: "Diretor Geral",
    Departamento: "Administração"
  });

  // --- 2. CRIAR PROFESSORES ---
  console.log("👨‍🏫 Criando professores...");
  
  for (let i = 0; i < 20; i++) {
    const nome = faker.person.fullName();
    // Gera email: nome.sobrenome@professor.com
    const nomeLimpo = nome.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").replace(/[^a-z0-9]/g, ".");
    const email = `${nomeLimpo.split('.')[0]}.${nomeLimpo.split('.').pop()}@professor.com`;
    
    const area = faker.helpers.arrayElement(AREAS);
    
    await criarUsuario(nome, email, "Professor", {
      Departamento: area,
      Formacao: faker.person.jobTitle(), // Ex: "Doutor em Ciências"
      Disciplinas: ["Matéria A", "Matéria B"] // Pode ser melhorado depois
    });
  }

  console.log("🏁 Concluído! Senha padrão para todos: 123456");
}

rodarScript();