FROM andyrbell/mountebank

ADD imposters /mb

EXPOSE 2525
EXPOSE 8000

CMD mb --configfile /mb/originServer.ejs --allowInjection