function executarAcao(acao) {
    document.form1.acao.value = acao;
    document.form1.submit();
}

// Limpa todos os inputs da tela
function estadoInicial() {
    var elements = document.querySelectorAll('input, select');
    if (elements) {
        elements.forEach(function (item) {
            if (item.name.startsWith("__") || item.type === 'submit') return;
            if (item.name === 'view') item.value = 'Index';

            var type = item.type;
            var tag = item.tagName.toLowerCase();


            if (type == 'text' || type == 'hidden' || type == 'password' || type == 'textarea') {
                item.value = '';
            } else if (type == 'checkbox' || type == 'radio') {
                item.checked = false;
            } else if (tag == 'select') {
                item.selectedIndex = -1;
            }
        });
    }
}