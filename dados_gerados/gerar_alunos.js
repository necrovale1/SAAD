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

// --- CONFIGURAÇÕES ---
// DICA: Comece com 50 ou 100 para testar. Se colocar 1000, vai demorar uns 10 minutos.
const QUANTIDADE_ALUNOS = 200; 
const SENHA_PADRAO = "123456";

// --- BANCO DE DADOS DE MATÉRIAS POR ÁREA ---
const MATERIAS_POR_AREA = {
  "TI": ["Lógica de Programação", "Banco de Dados", "Engenharia de Software", "Front-end", "Redes", "Segurança da Informação", "Cloud Computing", "Mobile", "UX/UI Design", "Inteligência Artificial"],
  "SAUDE": ["Anatomia Humana", "Fisiologia", "Saúde Pública", "Biossegurança", "Farmacologia", "Primeiros Socorros", "Patologia", "Ética na Saúde", "Microbiologia"],
  "GESTAO": ["Administração Geral", "Contabilidade", "Marketing", "Gestão de Pessoas", "Finanças", "Empreendedorismo", "Logística", "Economia", "Direito Empresarial"],
  "EXATAS": ["Cálculo I", "Física Geral", "Geometria Analítica", "Estatística", "Química Geral", "Desenho Técnico", "Resistência dos Materiais", "Álgebra Linear"],
  "AGRARIAS": ["Solos", "Botânica", "Zootecnia", "Mecanização Agrícola", "Agroecologia", "Topografia", "Gestão Rural", "Irrigação e Drenagem"],
  "HUMANAS": ["Sociologia", "Filosofia", "História da Arte", "Psicologia", "Antropologia", "Metodologia Científica", "Ética e Cidadania", "Comunicação e Expressão"],
  "INDUSTRIA": ["Automação", "Eletrônica Analógica", "Eletrônica Digital", "Processos de Fabricação", "Manutenção Industrial", "Instrumentação", "Segurança do Trabalho"],
  "IDIOMAS": ["Inglês Técnico", "Espanhol Básico", "Gramática", "Literatura", "Redação Técnica", "Interpretação de Texto"],
  "ARTE": ["História da Arte", "Teoria da Cor", "Composição Visual", "Desenho Artístico", "Computação Gráfica", "Projeto de Design"],
  "DIREITO": ["Direito Constitucional", "Direito Civil", "Direito Penal", "Teoria Geral do Processo", "Direito Trabalhista", "Direito Tributário"]
};

// --- LISTAS DE CURSOS ---
const CURSOS_SUPERIOR = [
  "Administração", "Análise e Desenvolvimento de Sistemas", "Arquitetura e Urbanismo", "Artes Visuais", "Biomedicina", 
  "Ciência da Computação", "Ciência de Dados e Inteligência Artificial", "Ciências Biológicas", "Ciências Contábeis", 
  "Ciências Econômicas", "Comércio Exterior", "Design", "Design de Interiores", "Direito", "Educação Física", 
  "Enfermagem", "Engenharia Agronômica", "Engenharia Ambiental", "Engenharia Civil", "Engenharia da Computação", 
  "Engenharia de Controle e Automação", "Engenharia de Produção", "Engenharia Elétrica", "Engenharia Mecânica", 
  "Engenharia Química", "Estética e Cosmética", "Farmácia", "Filosofia", "Fisioterapia", "Fonoaudiologia", 
  "Gastronomia", "Gestão Comercial", "Gestão da Qualidade", "Gestão de Recursos Humanos", "Gestão Financeira", 
  "História", "Jogos Digitais", "Jornalismo", "Letras: Português", "Logística", "Marketing", "Medicina Veterinária", 
  "Moda", "Nutrição", "Odontologia", "Pedagogia", "Processos Gerenciais", "Psicologia", "Publicidade e Propaganda", 
  "Química Industrial", "Relações Internacionais", "Relações Públicas", "Terapia Ocupacional"
];

const CURSOS_TECNICO = [
  "Técnico em Administração", "Técnico em Agenciamento de Viagem", "Técnico em Agricultura", "Técnico em Agronegócio", 
  "Técnico em Alimentos", "Técnico em Automação Industrial", "Técnico em Biotecnologia", "Técnico em Cafeicultura", 
  "Técnico em Canto", "Técnico em Comércio", "Técnico em Contabilidade", "Técnico em Dança", "Técnico em Desenvolvimento de Sistemas", 
  "Técnico em Design de Interiores", "Técnico em Design Gráfico", "Técnico em Edificações", "Técnico em Eletroeletrônica", 
  "Técnico em Eletrônica", "Técnico em Enfermagem", "Técnico em Eventos", "Técnico em Farmácia", "Técnico em Finanças", 
  "Técnico em Gastronomia", "Técnico em Guia de Turismo", "Técnico em Hospedagem", "Técnico em Informática", 
  "Técnico em Informática para Internet", "Técnico em Logística", "Técnico em Manutenção Automotiva", "Técnico em Marketing", 
  "Técnico em Mecânica", "Técnico em Mecatrônica", "Técnico em Meio Ambiente", "Técnico em Mineração", "Técnico em Nutrição e Dietética", 
  "Técnico em Paisagismo", "Técnico em Portos", "Técnico em Programação de Jogos Digitais", "Técnico em Química", 
  "Técnico em Recursos Humanos", "Técnico em Redes de Computadores", "Técnico em Saúde Bucal", "Técnico em Segurança do Trabalho", 
  "Técnico em Serviços Jurídicos", "Técnico em Transações Imobiliárias", "Técnico em Veterinária", "Técnico em Zootecnia"
];

// Função auxiliar para "adivinhar" as matérias baseadas no nome do curso
function getMateriasPorCurso(nomeCurso) {
  const nome = nomeCurso.toLowerCase();
  
  if (nome.includes("sistemas") || nome.includes("computação") || nome.includes("dados") || nome.includes("jogos") || nome.includes("informática") || nome.includes("digital")) return MATERIAS_POR_AREA.TI;
  if (nome.includes("enfermagem") || nome.includes("saúde") || nome.includes("medicina") || nome.includes("biomedicina") || nome.includes("nutrição") || nome.includes("farmácia") || nome.includes("terapia") || nome.includes("odontologia")) return MATERIAS_POR_AREA.SAUDE;
  if (nome.includes("administração") || nome.includes("gestão") || nome.includes("comércio") || nome.includes("marketing") || nome.includes("recursos humanos") || nome.includes("contábeis") || nome.includes("finanças") || nome.includes("logística") || nome.includes("publicidade")) return MATERIAS_POR_AREA.GESTAO;
  if (nome.includes("engenharia") || nome.includes("química") || nome.includes("física") || nome.includes("matemática") || nome.includes("edificações") || nome.includes("mecanica") || nome.includes("eletrica")) return MATERIAS_POR_AREA.EXATAS;
  if (nome.includes("agro") || nome.includes("veterinária") || nome.includes("zootecnia") || nome.includes("florestas") || nome.includes("cafeicultura")) return MATERIAS_POR_AREA.AGRARIAS;
  if (nome.includes("direito") || nome.includes("jurídicos")) return MATERIAS_POR_AREA.DIREITO;
  if (nome.includes("design") || nome.includes("artes") || nome.includes("moda") || nome.includes("teatro") || nome.includes("dança") || nome.includes("fotografia")) return MATERIAS_POR_AREA.ARTE;
  if (nome.includes("história") || nome.includes("filosofia") || nome.includes("letras") || nome.includes("pedagogia") || nome.includes("sociologia") || nome.includes("psicologia")) return MATERIAS_POR_AREA.HUMANAS;
  
  // Padrão se não achar nada específico
  return ["Introdução ao Curso", "Ética Profissional", "Metodologia", "Projeto Integrador"];
}

// Função para dar uma pausa (evitar erro de Rate Limit do Firebase)
const sleep = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

async function criarDadosCompletos() {
  console.log(`🚀 Iniciando a criação de ${QUANTIDADE_ALUNOS} alunos...`);
  console.log(`⚠️ Isso pode levar alguns minutos para evitar bloqueios.`);

  for (let i = 0; i < QUANTIDADE_ALUNOS; i++) {
    // 1. Sortear Nível (Superior ou Técnico)
    const nivel = faker.helpers.arrayElement(["Superior", "Técnico"]);
    const listaCursos = nivel === "Superior" ? CURSOS_SUPERIOR : CURSOS_TECNICO;
    const curso = faker.helpers.arrayElement(listaCursos);
    
    // 2. Pegar matérias compatíveis
    const materias = getMateriasPorCurso(curso);
    
    // 3. Gerar dados pessoais
    const nome = faker.person.fullName();
    // Limpeza de acentos para email
    const nomeLimpo = nome.normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().replace(/[^a-z0-9 ]/g, ""); 
    const primeiroNome = nomeLimpo.split(' ')[0];
    const sobrenome = nomeLimpo.split(' ')[1] || "aluno";
    // Adiciona numero aleatório no email para garantir que seja único
    const email = `${primeiroNome}.${sobrenome}${faker.number.int({min:1, max:999})}@aluno.com`;
    
    const rm = faker.number.int({ min: 10000, max: 99999 }).toString();
    
    try {
      // 4. Criar Auth
      const userRecord = await auth.createUser({
        email: email,
        password: SENHA_PADRAO,
        displayName: nome,
      });

      // 5. Criar User no Firestore
      await db.collection('users').doc(userRecord.uid).set({
        Nome: nome,
        Email: email,
        RegistroAcademico: rm,
        UserType: "Aluno",
        Curso: curso,
        Nivel: nivel
      });

      // 6. Criar Presenças (entre 8 e 20 registros por aluno para ficar bem cheio)
      const numPresencas = faker.number.int({ min: 8, max: 20 });
      
      const batch = db.batch(); // Usando Batch para ser mais eficiente no banco

      for (let j = 0; j < numPresencas; j++) {
        const materiaSorteada = faker.helpers.arrayElement(materias);
        const dataRecente = faker.date.recent({ days: 90 });
        
        const dia = dataRecente.toISOString().split('T')[0];
        const horaEntrada = faker.number.int({min: 7, max: 21}); // Aulas manhã, tarde ou noite
        const horaSaida = horaEntrada + faker.number.int({min: 1, max: 4});
        
        const docRef = db.collection('presencas').doc();
        batch.set(docRef, {
            RegistroAcademico: rm,
            Materia: materiaSorteada,
            Data: dia,
            HoraEntrada: `${horaEntrada}:00`,
            HoraSaida: `${horaSaida}:00`,
            Status: faker.helpers.arrayElement(["Presente", "Presente", "Presente", "Ausente", "Justificado"])
        });
      }
      
      await batch.commit(); // Salva todas as presenças de uma vez

      console.log(`✅ [${i+1}/${QUANTIDADE_ALUNOS}] ${nome} | ${curso}`);

      // Pequena pausa a cada 10 criações para o Firebase não bloquear
      if (i % 10 === 0) await sleep(500); 

    } catch (error) {
      console.error(`❌ Erro ao criar ${email}:`, error.message);
    }
  }

  console.log("🏁 Todos os alunos foram criados com sucesso!");
}

criarDadosCompletos();